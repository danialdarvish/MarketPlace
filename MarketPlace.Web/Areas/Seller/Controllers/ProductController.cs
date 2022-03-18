using System.Threading.Tasks;
using MarketPlace.Application.Services.Interfaces;
using MarketPlace.DataLayer.DTOs.Product;
using MarketPlace.Web.Http;
using MarketPlace.Web.PresentationExtensions;
using Microsoft.AspNetCore.Http;
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

        [HttpGet("products-list")]
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
            ViewBag.Categories = await _productService.GetAllActiveProductCategories();
            return View();
        }

        [HttpPost("create-product"), ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProduct(CreateProductDto product, IFormFile productImage)
        {
            if (ModelState.IsValid)
            {
                // todo: Create product
            }

            ViewBag.Categories = await _productService.GetAllProductCategoriesByParentId(null);
            return View(product);
        }

        #endregion

        #region Product categories

        [HttpGet("product-categories/{parentId}")]
        public async Task<IActionResult> GetProductCategoriesByParent(long parentId)
        {
            var categories = await _productService.GetAllProductCategoriesByParentId(parentId);

            return JsonResponseStatus.SendStatus(JsonResponseStatusType.Success, 
                "اطلاعات دسته بندی ها", categories);
        }

        #endregion
    }
}
