using System;
using System.Threading.Tasks;
using MarketPlace.DataLayer.DTOs.Product_Discount;

namespace MarketPlace.Application.Services.Interfaces
{
    public interface IProductDiscountService : IAsyncDisposable
    {
        #region Product discount

        Task<FilterProductDiscountDto> FilterProductDiscount(FilterProductDiscountDto filter);
        Task<CreateDiscountResult> CreateProductDiscount(CreateProductDiscountDto discount, long sellerId);

        #endregion
    }
}
