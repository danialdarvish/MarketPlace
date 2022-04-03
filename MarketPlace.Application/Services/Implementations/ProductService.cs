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
        private readonly IGenericRepository<ProductCategory> _productCategoryRepository;
        private readonly IGenericRepository<ProductSelectedCategory> _productSelectedCategoryRepository;

        public ProductService(IGenericRepository<Product> productRepository, IGenericRepository<ProductColor> productColorRepository, IGenericRepository<ProductCategory> productCategoryRepository, IGenericRepository<ProductSelectedCategory> productSelectedCategoryRepository)
        {
            _productRepository = productRepository;
            _productColorRepository = productColorRepository;
            _productCategoryRepository = productCategoryRepository;
            _productSelectedCategoryRepository = productSelectedCategoryRepository;
        }

        #endregion

        #region Products

        public async Task<FilterProductDto> FilterProducts(FilterProductDto filter)
        {
            var query = _productRepository.GetQuery().AsQueryable();

            #region State

            switch (filter.FilterProductState)
            {
                case FilterProductState.UnderProgress:
                    query = query.Where(x => x.ProductAcceptanceState == ProductAcceptanceState.UnderProgress);
                    break;
                case FilterProductState.Accepted:
                    query = query.Where(x => x.ProductAcceptanceState == ProductAcceptanceState.Accepted);
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

            #endregion

            #region Filter

            if (!string.IsNullOrEmpty(filter.Title))
                query = query.Where(x => EF.Functions.Like(x.Title, $"%{filter.Title}%"));
            if (filter.SellerId != null && filter.SellerId != 0)
                query = query.Where(x => x.SellerId == filter.SellerId.Value);

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
                        Price = x.Price
                    }).ToListAsync(),
                SelectedCategories = await _productSelectedCategoryRepository.GetQuery()
                    .Where(x => x.ProductId == productId && !x.IsDelete)
                    .Select(x => x.ProductCategoryId).ToListAsync()
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
            var productSelectedColors = new List<ProductColor>();

            foreach (var productColor in colors)
            {
                productSelectedColors.Add(new ProductColor
                {
                    ColorName = productColor.ColorName,
                    Price = productColor.Price,
                    ProductId = productId
                });
            }

            await _productColorRepository.AddRangeEntity(productSelectedColors);
        }

        public async Task RemoveAllProductSelectedColors(long productId)
        {
            _productColorRepository
                .DeletePermanentEntities(await _productColorRepository.GetQuery()
                    .Where(x => x.ProductId == productId).ToListAsync());
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

        #endregion

        #region Dispose

        public async ValueTask DisposeAsync()
        {
            await _productRepository.DisposeAsync();
            await _productCategoryRepository.DisposeAsync();
            await _productSelectedCategoryRepository.DisposeAsync();
        }

        #endregion
    }
}
