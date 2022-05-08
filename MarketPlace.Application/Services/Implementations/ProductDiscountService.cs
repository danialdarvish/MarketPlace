using System;
using System.Linq;
using System.Threading.Tasks;
using MarketPlace.Application.Services.Interfaces;
using MarketPlace.Application.Utils;
using MarketPlace.DataLayer.DTOs.Paging;
using MarketPlace.DataLayer.DTOs.Product_Discount;
using MarketPlace.DataLayer.Entities.Products;
using MarketPlace.DataLayer.Repository;
using Microsoft.EntityFrameworkCore;

namespace MarketPlace.Application.Services.Implementations
{
    public class ProductDiscountService : IProductDiscountService
    {
        #region Constructor

        private readonly IGenericRepository<Product> _productRepository;
        private readonly IGenericRepository<ProductDiscount> _productDiscountRepository;
        private readonly IGenericRepository<ProductDiscountUse> _productDiscountUseRepository;

        public ProductDiscountService(IGenericRepository<Product> productRepository, IGenericRepository<ProductDiscount> productDiscountRepository, IGenericRepository<ProductDiscountUse> productDiscountUseRepository)
        {
            _productRepository = productRepository;
            _productDiscountRepository = productDiscountRepository;
            _productDiscountUseRepository = productDiscountUseRepository;
        }

        #endregion

        #region Product discount

        public async Task<FilterProductDiscountDto> FilterProductDiscount(FilterProductDiscountDto filter)
        {
            var query = _productDiscountRepository.GetQuery()
                .Include(x => x.Product)
                .AsQueryable();

            #region Filter

            if (filter.ProductId != null && filter.ProductId != 0)
                query = query.Where(x => x.ProductId == filter.ProductId.Value);
            if (filter.SellerId != null && filter.SellerId != 0)
                query = query.Where(x => x.Product.SellerId == filter.SellerId.Value);

            #endregion

            #region Paging

            var pager = Pager.Build(filter.PageId, await query.CountAsync(), filter.TakeEntity,
                filter.ShowHowManyPageAfterAndBefore);
            var allEntities = await query.Paging(pager).ToListAsync();

            #endregion

            return filter.SetPaging(pager).SetDiscounts(allEntities);
        }

        public async Task<CreateDiscountResult> CreateProductDiscount(CreateProductDiscountDto discount, long sellerId)
        {
            var product = await _productRepository.GetEntityById(discount.ProductId);

            if (product == null) return CreateDiscountResult.ProductNotFound;
            if (product.SellerId != sellerId) return CreateDiscountResult.ProductIsNotForSeller;

            var newDiscount = new ProductDiscount
            {
                ProductId = product.Id,
                DiscountNumber = discount.DiscountNumber,
                Percentage = discount.Percentage,
                ExpireDate = discount.ExpireDate.ToMiladiDateTime()
            };

            await _productDiscountRepository.AddEntity(newDiscount);
            await _productDiscountRepository.SaveChanges();

            return CreateDiscountResult.Success;
        }

        #endregion

        #region Dispose

        public async ValueTask DisposeAsync()
        {
            await _productDiscountRepository.DisposeAsync();
            await _productDiscountUseRepository.DisposeAsync();
        }

        #endregion
    }
}
