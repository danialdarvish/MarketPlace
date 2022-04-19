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
        Task<ProductDetailDto> GetProductDetailById(long productId);

        #endregion

        #region ProductCategory

        Task<List<ProductCategory>> GetAllProductCategoriesByParentId(long? parentId);
        Task<List<ProductCategory>> GetAllActiveProductCategories();

        #endregion

        #region ProductGallery

        Task<List<ProductGallery>> GetAllProductGalleries(long productId);
        Task<Product> GetProductBySellerOwnerId(long productId, long userId);
        Task<List<ProductGallery>> GetAllProductGalleriesInSellerPanel(long productId, long sellerId);
        Task<CreateOrEditProductGalleryResult> CreateProductGallery(CreateOrEditProductGalleryDto gallery, long productId, long sellerId);
        Task<CreateOrEditProductGalleryDto> GetProductGalleryForEdit(long galleryId, long sellerId);
        Task<CreateOrEditProductGalleryResult> EditProductGallery(long galleryId, long sellerId, CreateOrEditProductGalleryDto gallery);

        #endregion

        #region ProductFeature

        Task CreateProductFeatures(List<CreateProductFeatureDto> features, long productId);
        Task RemoveAllProductFeatures(long productId);

        #endregion
    }
}
