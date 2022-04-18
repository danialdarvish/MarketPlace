using System.Threading.Tasks;
using MarketPlace.Application.Services.Interfaces;
using MarketPlace.DataLayer.DTOs.Product;
using Microsoft.AspNetCore.Mvc;

namespace MarketPlace.Web.Controllers
{
    public class ProductController : SiteBaseController
    {
        #region Constructor

        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        #endregion

        #region Filter products

        [HttpGet("products")]
        [HttpGet("products/{Category}")]
        public async Task<IActionResult> FilterProducts(FilterProductDto filter)
        {
            filter.TakeEntity = 9;
            filter.FilterProductState = FilterProductState.Accepted;
            filter = await _productService.FilterProducts(filter);
            ViewBag.ProductCategories = await _productService.GetAllActiveProductCategories();
            if (filter.PageId > filter.GetLastPage() && filter.GetLastPage() != 0) return NotFound();

            return View(filter);
        }

        #endregion

        #region Product detail

        [HttpGet("products/{productId}/{title}")]
        public async Task<IActionResult> ProductDetail(long productId, string title)
        {
            var product = await _productService.GetProductDetailById(productId);
            if (product == null) return NotFound();

            return View(product);
        }

        #endregion
    }
}
