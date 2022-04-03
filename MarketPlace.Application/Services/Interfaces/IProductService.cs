using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MarketPlace.DataLayer.DTOs.Common;
using MarketPlace.DataLayer.DTOs.Product;
using MarketPlace.DataLayer.Entities.Products;
using Microsoft.AspNetCore.Http;

namespace MarketPlace.Application.Services.Interfaces
{
    public interface IProductService : IAsyncDisposable
    {
        #region Products

        Task<FilterProductDto> FilterProducts(FilterProductDto filter);
        Task<CreateProductResult> CreateProduct(CreateProductDto product, long sellerId, IFormFile productImage);
        Task<bool> AcceptSellerProduct(long productId);
        Task<bool> RejectSellerProduct(RejectItemDto reject);
        Task<EditProductDto> GetProductForEdit(long productId);
        Task<EditProductResult> EditSellerProduct(EditProductDto product, long userId, IFormFile productImage);
        Task AddProductSelectedCategories(long productId, List<long> selectedCategories);
        Task RemoveAllProductSelectedCategories(long productId);
        Task AddProductSelectedColors(long productId, List<CreateProductColorDto> colors);
        Task RemoveAllProductSelectedColors(long productId);

        #endregion

        #region ProductCategory

        Task<List<ProductCategory>> GetAllProductCategoriesByParentId(long? parentId);
        Task<List<ProductCategory>> GetAllActiveProductCategories();

        #endregion
    }
}
