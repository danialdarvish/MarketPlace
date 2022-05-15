using System.Threading.Tasks;
using MarketPlace.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MarketPlace.Web.ViewComponents
{
    public class ProductSliderViewComponent : ViewComponent
    {
        #region Constructor

        private readonly IProductService _productService;

        public ProductSliderViewComponent(IProductService productService)
        {
            _productService = productService;
        }

        #endregion

        #region Body

        public async Task<IViewComponentResult> InvokeAsync(string categoryName)
        {
            var category = await _productService.GetProductCategoryByUrlName(categoryName);
            var products = await _productService.GetCategoryProductsByCategoryName(categoryName);
            ViewBag.title = category?.Title;

            return View("ProductSlider", products);
        }

        #endregion
    }
}
