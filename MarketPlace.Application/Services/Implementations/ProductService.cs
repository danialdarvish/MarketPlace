using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MarketPlace.Application.Extensions;
using MarketPlace.Application.Services.Interfaces;
using MarketPlace.Application.Utils;
using MarketPlace.DataLayer.DTOs.Common;
using MarketPlace.DataLayer.DTOs.Paging;
using MarketPlace.DataLayer.DTOs.Product;
using MarketPlace.DataLayer.Entities.Products;
using MarketPlace.DataLayer.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace MarketPlace.Application.Services.Implementations
{
    public class ProductService : IProductService
    {
        #region Constructor

        private readonly IGenericRepository<Product> _productRepository;
        private readonly IGenericRepository<ProductColor> _productColorRepository;
        private readonly IGenericRepository<ProductGallery> _productGalleryRepository;
        private readonly IGenericRepository<ProductFeature> _productFeatureRepository;
        private readonly IGenericRepository<ProductCategory> _productCategoryRepository;
        private readonly IGenericRepository<ProductDiscount> _productDiscountRepository;
        private readonly IGenericRepository<ProductSelectedCategory> _productSelectedCategoryRepository;

        public ProductService(IGenericRepository<Product> productRepository, IGenericRepository<ProductColor> productColorRepository, IGenericRepository<ProductGallery> productGalleryRepository, IGenericRepository<ProductFeature> productFeatureRepository, IGenericRepository<ProductCategory> productCategoryRepository, IGenericRepository<ProductDiscount> productDiscountRepository, IGenericRepository<ProductSelectedCategory> productSelectedCategoryRepository)
        {
            _productRepository = productRepository;
            _productColorRepository = productColorRepository;
            _productGalleryRepository = productGalleryRepository;
            _productFeatureRepository = productFeatureRepository;
            _productCategoryRepository = productCategoryRepository;
            _productDiscountRepository = productDiscountRepository;
            _productSelectedCategoryRepository = productSelectedCategoryRepository;
        }

        #endregion

        #region Products

        public async Task<FilterProductDto> FilterProducts(FilterProductDto filter)
        {
            var query = _productRepository.GetQuery()
                .Include(x => x.ProductSelectedCategories)
                .ThenInclude(x => x.ProductCategory)
                .AsQueryable();

            var expensiveProduct = await query.OrderByDescending(x => x.Price).FirstOrDefaultAsync();
            filter.FilterMaxPrice = expensiveProduct.Price;

            #region State

            switch (filter.FilterProductState)
            {
                case FilterProductState.UnderProgress:
                    query = query.Where(x => x.ProductAcceptanceState == ProductAcceptanceState.UnderProgress);
                    break;
                case FilterProductState.Accepted:
                    query = query.Where(x => x.ProductAcceptanceState == ProductAcceptanceState.Accepted && x.IsActive);
                    break;
                case FilterProductState.Rejected:
                    query = query.Where(x => x.ProductAcceptanceState == ProductAcceptanceState.Rejected);
                    break;
                case FilterProductState.Active:
                    query = query.Where(x => x.IsActive && x.ProductAcceptanceState == ProductAcceptanceState.Accepted);
                    break;
                case FilterProductState.NotActive:
                    query = query.Where(x => !x.IsActive && x.ProductAcceptanceState == ProductAcceptanceState.Accepted);
                    break;
                case FilterProductState.All:
                    break;
            }

            switch (filter.FilterProductOrderBy)
            {
                case FilterProductOrderBy.CreateDate_Des:
                    query = query.OrderByDescending(x => x.CreateDate);
                    break;
                case FilterProductOrderBy.CreateDate_Asc:
                    query = query.OrderBy(x => x.CreateDate);
                    break;
                case FilterProductOrderBy.Price_Des:
                    query = query.OrderByDescending(x => x.Price);
                    break;
                case FilterProductOrderBy.Price_Asc:
                    query = query.OrderBy(x => x.Price);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            #endregion

            #region Filter

            if (!string.IsNullOrEmpty(filter.Title))
                query = query.Where(x => EF.Functions.Like(x.Title, $"%{filter.Title}%"));
            if (filter.SellerId != null && filter.SellerId != 0)
                query = query.Where(x => x.SellerId == filter.SellerId.Value);
            if (filter.SelectedMaxPrice == 0) filter.SelectedMaxPrice = expensiveProduct.Price;

            query = query.Where(x => x.Price >= filter.SelectedMinPrice);
            query = query.Where(x => x.Price <= filter.SelectedMaxPrice);

            if (!string.IsNullOrEmpty(filter.Category))
                query = query.Where(x => x.ProductSelectedCategories.Any(s => s.ProductCategory.UrlName == filter.Category));

            #endregion

            #region Paging

            var pager = Pager.Build(filter.PageId, await query.CountAsync(), filter.TakeEntity,
                filter.ShowHowManyPageAfterAndBefore);
            var allEntities = await query.Paging(pager).ToListAsync();

            #endregion

            return filter.SetProducts(allEntities).SetPaging(pager);
        }

        public async Task<CreateProductResult> CreateProduct(CreateProductDto product, long sellerId, IFormFile productImage)
        {
            if (productImage == null) return CreateProductResult.HasNoImage;

            var imageName = Guid.NewGuid().ToString("N") +
                            Path.GetExtension(productImage.FileName);

            var result = productImage.AddImageToServer(imageName, PathExtension.ProductImageServer,
                150, 150, PathExtension.ProductImageThumbnailServer);

            if (!result) return CreateProductResult.Error;

            // Create product
            var newProduct = new Product
            {
                Title = product.Title,
                Price = product.Price,
                ShortDescription = product.ShortDescription,
                Description = product.Description,
                IsActive = product.IsActive,
                SellerId = sellerId,
                ImageName = imageName,
                ProductAcceptanceState = ProductAcceptanceState.UnderProgress
            };

            await _productRepository.AddEntity(newProduct);
            await _productRepository.SaveChanges();

            // Create product categories

            await AddProductSelectedCategories(newProduct.Id, product.SelectedCategories);
            await _productSelectedCategoryRepository.SaveChanges();

            // Create product colors

            await AddProductSelectedColors(newProduct.Id, product.ProductColors);
            await _productColorRepository.SaveChanges();

            return CreateProductResult.Success;
        }

        public async Task<bool> AcceptSellerProduct(long productId)
        {
            var product = await _productRepository.GetEntityById(productId);
            if (product != null)
            {
                product.ProductAcceptanceState = ProductAcceptanceState.Accepted;
                product.ProductAcceptOrRejectDescription = $"محصول مورد نظر در تاریخ {DateTime.Now.ToShamsi()} مورد تایید سایت قرار گرفت";
                _productRepository.EditEntity(product);
                await _productRepository.SaveChanges();
                return true;
            }

            return false;
        }

        public async Task<bool> RejectSellerProduct(RejectItemDto reject)
        {
            var product = await _productRepository.GetEntityById(reject.Id);
            if (product != null)
            {
                product.ProductAcceptanceState = ProductAcceptanceState.Rejected;
                product.ProductAcceptOrRejectDescription = reject.RejectMessage;

                _productRepository.EditEntity(product);
                await _productRepository.SaveChanges();

                return true;
            }

            return false;
        }

        public async Task<EditProductDto> GetProductForEdit(long productId)
        {
            var product = await _productRepository.GetEntityById(productId);
            if (product == null) return null;

            return new EditProductDto
            {
                Id = product.Id,
                ShortDescription = product.ShortDescription,
                Description = product.Description,
                Price = product.Price,
                ImageName = product.ImageName,
                IsActive = product.IsActive,
                Title = product.Title,
                ProductColors = await _productColorRepository.GetQuery()
                    .Where(x => x.ProductId == productId && !x.IsDelete)
                    .Select(x => new CreateProductColorDto
                    {
                        ColorName = x.ColorName,
                        Price = x.Price,
                        ColorCode = x.ColorCode
                    }).ToListAsync(),
                SelectedCategories = await _productSelectedCategoryRepository.GetQuery()
                    .Where(x => x.ProductId == productId && !x.IsDelete)
                    .Select(x => x.ProductCategoryId).ToListAsync(),
                ProductFeatures = await _productFeatureRepository.GetQuery()
                    .Where(x => x.ProductId == productId && !x.IsDelete).Select(x => new CreateProductFeatureDto
                    {
                        Feature = x.FeatureTitle,
                        FeatureValue = x.FeatureValue,
                    }).ToListAsync()
            };
        }

        public async Task<EditProductResult> EditSellerProduct(EditProductDto product, long userId, IFormFile productImage)
        {
            var mainProduct = await _productRepository.GetQuery()
                .Include(x => x.Seller)
                .FirstOrDefaultAsync(x => x.Id == product.Id);
            if (mainProduct == null) return EditProductResult.NotFound;
            if (mainProduct.Seller.UserId != userId) return EditProductResult.NotForUser;

            mainProduct.Title = product.Title;
            mainProduct.ShortDescription = product.ShortDescription;
            mainProduct.Price = product.Price;
            mainProduct.Description = product.Description;
            mainProduct.IsActive = product.IsActive;
            mainProduct.ProductAcceptanceState = ProductAcceptanceState.UnderProgress;

            if (productImage != null)
            {
                var imageName = Guid.NewGuid().ToString("N") +
                                Path.GetExtension(productImage.FileName);

                var result = productImage.AddImageToServer(imageName, PathExtension.ProductImageServer,
                    150, 150, PathExtension.ProductImageThumbnailServer, mainProduct.ImageName);

                if (result) mainProduct.ImageName = imageName;

            }

            //_productRepository.EditEntity(mainProduct);
            //await _productRepository.SaveChanges();

            // Product categories
            await RemoveAllProductSelectedCategories(product.Id);
            await AddProductSelectedCategories(product.Id, product.SelectedCategories);
            await _productSelectedCategoryRepository.SaveChanges();

            // Product colors
            await RemoveAllProductSelectedColors(product.Id);
            await AddProductSelectedColors(product.Id, product.ProductColors);
            await _productColorRepository.SaveChanges();

            // Product features
            await RemoveAllProductFeatures(product.Id);
            await CreateProductFeatures(product.ProductFeatures, product.Id);
            await _productFeatureRepository.SaveChanges();

            return EditProductResult.Success;
        }

        public async Task AddProductSelectedCategories(long productId, List<long> selectedCategories)
        {
            var productSelectedCategories = new List<ProductSelectedCategory>();
            foreach (var categoryId in selectedCategories)
            {
                productSelectedCategories.Add(new ProductSelectedCategory
                {
                    ProductCategoryId = categoryId,
                    ProductId = productId
                });
            }

            await _productSelectedCategoryRepository.AddRangeEntity(productSelectedCategories);
        }

        public async Task RemoveAllProductSelectedCategories(long productId)
        {
            _productSelectedCategoryRepository
                .DeletePermanentEntities(await _productSelectedCategoryRepository.GetQuery()
                    .Where(x => x.ProductId == productId).ToListAsync());
        }

        public async Task AddProductSelectedColors(long productId, List<CreateProductColorDto> colors)
        {
            if (colors != null && colors.Any())
            {
                var productSelectedColors = new List<ProductColor>();

                foreach (var productColor in colors)
                {
                    if (productSelectedColors.All(x => x.ColorName != productColor.ColorName))
                    {
                        productSelectedColors.Add(new ProductColor
                        {
                            ColorName = productColor.ColorName,
                            ColorCode = productColor.ColorCode,
                            Price = productColor.Price,
                            ProductId = productId,
                        });
                    }
                }

                await _productColorRepository.AddRangeEntity(productSelectedColors);
            }
        }

        public async Task RemoveAllProductSelectedColors(long productId)
        {
            _productColorRepository
                .DeletePermanentEntities(await _productColorRepository.GetQuery()
                    .Where(x => x.ProductId == productId).ToListAsync());
        }

        public async Task<ProductDetailDto> GetProductDetailById(long productId)
        {
            var product = await _productRepository.GetQuery()
                .Include(x => x.Seller)
                .ThenInclude(x => x.User)
                .Include(x => x.ProductSelectedCategories)
                .ThenInclude(x => x.ProductCategory)
                .Include(x => x.ProductGalleries)
                .Include(x => x.ProductColors)
                .Include(x => x.ProductFeatures)
                .FirstOrDefaultAsync(x => x.Id == productId);
            if (product == null) return null;

            var selectedCategoriesIds = product.ProductSelectedCategories
                .Select(x => x.ProductCategoryId).ToList();

            return new ProductDetailDto
            {
                ProductId = productId,
                Title = product.Title,
                Price = product.Price,
                ImageName = product.ImageName,
                Description = product.Description,
                ShortDescription = product.ShortDescription,
                Seller = product.Seller,
                SellerId = product.SellerId,
                ProductCategories = product.ProductSelectedCategories.Select(x => x.ProductCategory).ToList(),
                ProductGalleries = product.ProductGalleries.ToList(),
                ProductColors = product.ProductColors.ToList(),
                ProductFeatures = product.ProductFeatures.ToList(),
                RelatedProducts = await _productRepository.GetQuery()
                    .Include(x => x.ProductSelectedCategories)
                        .Where(x => x.ProductSelectedCategories
                            .Any(s => selectedCategoriesIds
                                .Contains(s.ProductCategoryId)) && x.Id != productId && x.ProductAcceptanceState == ProductAcceptanceState.Accepted)
                                    .ToListAsync()
            };
        }

        public async Task<List<Product>> FilterProductsForSellerByProductName(long sellerId, string productName)
        {
            return await _productRepository.GetQuery()
                .Where(x => x.SellerId == sellerId && EF.Functions
                    .Like(x.Title, $"%{productName}%"))
                .ToListAsync();
        }

        public async Task<List<ProductDiscount>> GetAllOffProducts(int take)
        {
            return await _productDiscountRepository.GetQuery()
                .Include(x => x.Product)
                .Where(x => x.ExpireDate >= DateTime.Now)
                .OrderByDescending(x => x.ExpireDate)
                .Skip(0)
                .Take(take)
                .Distinct().ToListAsync();
        }

        #endregion

        #region ProductCategory

        public async Task<List<ProductCategory>> GetAllProductCategoriesByParentId(long? parentId)
        {
            if (parentId is null or 0)
                return await _productCategoryRepository.GetQuery()
                    .Where(x => !x.IsDelete && x.IsActive && x.ParentId == null)
                    .ToListAsync();

            return await _productCategoryRepository.GetQuery()
                .Where(x => !x.IsDelete && x.IsActive && x.ParentId == parentId)
                .ToListAsync();
        }

        public async Task<List<ProductCategory>> GetAllActiveProductCategories()
        {
            return await _productCategoryRepository.GetQuery()
                .Where(x => x.IsActive && !x.IsDelete).ToListAsync();
        }

        public async Task<List<Product>> GetCategoryProductsByCategoryName(string categoryName, int count = 12)
        {
            var category = await _productCategoryRepository.GetQuery()
                .FirstOrDefaultAsync(x => x.UrlName == categoryName);
            if (category == null) return null;

            return await _productSelectedCategoryRepository.GetQuery()
                .Include(x => x.Product)
                .Where(x => x.ProductCategoryId == category.Id &&
                            x.Product.IsActive && !x.Product.IsDelete)
                .Select(x => x.Product)
                .OrderByDescending(x => x.CreateDate)
                .Distinct().Take(count).ToListAsync();
        }

        public async Task<ProductCategory> GetProductCategoryByUrlName(string productCategoryUrlName)
        {
            return await _productCategoryRepository.GetQuery()
                .FirstOrDefaultAsync(x => x.UrlName == productCategoryUrlName);
        }

        #endregion

        #region ProductGallery

        public async Task<List<ProductGallery>> GetAllProductGalleries(long productId)
        {
            return await _productGalleryRepository.GetQuery()
                .Where(x => x.ProductId == productId)
                .ToListAsync();
        }

        public async Task<Product> GetProductBySellerOwnerId(long productId, long userId)
        {
            return await _productRepository.GetQuery()
                .Include(x => x.Seller)
                .FirstOrDefaultAsync(x => x.Id == productId && x.Seller.UserId == userId);
        }

        public async Task<List<ProductGallery>> GetAllProductGalleriesInSellerPanel(long productId, long sellerId)
        {
            return await _productGalleryRepository.GetQuery()
                .Include(x => x.Product)
                .Where(x => x.Product.SellerId == sellerId && x.ProductId == productId)
                .ToListAsync();
        }

        public async Task<CreateOrEditProductGalleryResult> CreateProductGallery(CreateOrEditProductGalleryDto gallery, long productId, long sellerId)
        {
            var product = await _productRepository.GetEntityById(productId);
            if (product == null) return CreateOrEditProductGalleryResult.GalleryNotFound;
            if (product.SellerId != sellerId) return CreateOrEditProductGalleryResult.NotForUserProduct;
            if (gallery.Image == null || !gallery.Image.IsImage()) return CreateOrEditProductGalleryResult.ImageIsNull;

            var imageName = Guid.NewGuid().ToString("N") + Path.GetExtension(gallery.Image.FileName);
            gallery.Image.AddImageToServer(imageName, PathExtension.ProductGalleryImageServer,
                100, 100, PathExtension.ProductGalleryImageThumbnailServer);

            await _productGalleryRepository.AddEntity(new ProductGallery
            {
                ProductId = productId,
                ImageName = imageName,
                DisplayPriority = gallery.DisplayPriority
            });
            await _productGalleryRepository.SaveChanges();

            return CreateOrEditProductGalleryResult.Success;
        }

        public async Task<CreateOrEditProductGalleryDto> GetProductGalleryForEdit(long galleryId, long sellerId)
        {
            var gallery = await _productGalleryRepository.GetQuery()
                .Include(x => x.Product)
                .FirstOrDefaultAsync(x => x.Id == galleryId && x.Product.SellerId == sellerId);

            if (gallery == null) return null;

            return new CreateOrEditProductGalleryDto
            {
                DisplayPriority = gallery.DisplayPriority,
                ImageName = gallery.ImageName
            };
        }

        public async Task<CreateOrEditProductGalleryResult> EditProductGallery(long galleryId, long sellerId, CreateOrEditProductGalleryDto gallery)
        {
            var mainGallery = await _productGalleryRepository.GetQuery()
                .Include(x => x.Product)
                .FirstOrDefaultAsync(x => x.Id == galleryId);

            if (mainGallery == null) return CreateOrEditProductGalleryResult.GalleryNotFound;
            if (mainGallery.Product.SellerId != sellerId) return CreateOrEditProductGalleryResult.NotForUserProduct;
            if (gallery.Image != null && gallery.Image.IsImage())
            {
                var imageName = Guid.NewGuid().ToString("N") + Path.GetExtension(gallery.Image.FileName);
                var result = gallery.Image.AddImageToServer(imageName, PathExtension.ProductGalleryImageServer,
                    100, 100, PathExtension.ProductGalleryImageThumbnailServer, mainGallery.ImageName);

                mainGallery.ImageName = imageName;
            }

            mainGallery.DisplayPriority = gallery.DisplayPriority;

            _productGalleryRepository.EditEntity(mainGallery);
            await _productGalleryRepository.SaveChanges();

            return CreateOrEditProductGalleryResult.Success;
        }

        #endregion

        #region ProductFeature

        public async Task CreateProductFeatures(List<CreateProductFeatureDto> features, long productId)
        {
            var newFeatures = new List<ProductFeature>();
            if (features != null && features.Any())
            {
                foreach (var feature in features)
                {
                    newFeatures.Add(new ProductFeature
                    {
                        ProductId = productId,
                        FeatureTitle = feature.Feature,
                        FeatureValue = feature.FeatureValue
                    });
                }

                await _productFeatureRepository.AddRangeEntity(newFeatures);
                await _productFeatureRepository.SaveChanges();
            }
        }

        public async Task RemoveAllProductFeatures(long productId)
        {
            var productFeatures = await _productFeatureRepository.GetQuery()
                .Where(x => x.ProductId == productId).ToListAsync();

            _productFeatureRepository.DeletePermanentEntities(productFeatures);
            await _productFeatureRepository.SaveChanges();
        }

        #endregion

        #region Dispose

        public async ValueTask DisposeAsync()
        {
            await _productRepository.DisposeAsync();
            await _productCategoryRepository.DisposeAsync();
            await _productSelectedCategoryRepository.DisposeAsync();
            await _productFeatureRepository.DisposeAsync();
            await _productColorRepository.DisposeAsync();
            await _productGalleryRepository.DisposeAsync();
        }

        #endregion
    }
}
