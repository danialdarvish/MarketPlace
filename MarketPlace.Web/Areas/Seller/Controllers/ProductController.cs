using System.Threading.Tasks;
using MarketPlace.Application.Services.Interfaces;
using MarketPlace.DataLayer.DTOs.Product;
using MarketPlace.Web.PresentationExtensions;
using Microsoft.AspNetCore.Mvc;

namespace MarketPlace.Web.Areas.Seller.Controllers
{
    public class ProductController : SellerBaseController
    {
        #region Constructor

        private readonly ISellerService _sellerService;
        private readonly IProductService _productService;

        public ProductController(ISellerService sellerService, IProductService productService)
        {
            _sellerService = sellerService;
            _productService = productService;
        }

        #endregion

        #region List

        [HttpGet("products")]
        public async Task<IActionResult> Index(FilterProductDto filter)
        {
            var seller = await _sellerService.GetLastActiveSellerByUserId(User.GetUserId());
            filter.SellerId = seller.Id;
            filter.FilterProductState = FilterProductState.Active;
            return View(await _productService.FilterProducts(filter));
        }

        #endregion

        #region Create product

        [HttpGet("create-product")]
        public async Task<IActionResult> CreateProduct()
        {
            ViewBag.MainCategories = await _productService.GetAllProductCategoriesByParentId(null);
            return View();
        }

        [HttpPost("create-product"), ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProduct(CreateProductDto product)
        {
            if (ModelState.IsValid)
            {
                // todo: Create product
            }

            ViewBag.MainCategories = await _productService.GetAllProductCategoriesByParentId(null);
            return View(product);
        }
        #endregion
    }
}
