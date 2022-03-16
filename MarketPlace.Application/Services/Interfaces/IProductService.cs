using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MarketPlace.DataLayer.DTOs.Product;
using MarketPlace.DataLayer.Entities.Products;

namespace MarketPlace.Application.Services.Interfaces
{
    public interface IProductService : IAsyncDisposable
    {
        #region Products

        Task<FilterProductDto> FilterProducts(FilterProductDto filter);

        #endregion

        #region ProductCategory

        Task<List<ProductCategory>> GetAllProductCategoriesByParentId(long? parentId);

        #endregion
    }
}
