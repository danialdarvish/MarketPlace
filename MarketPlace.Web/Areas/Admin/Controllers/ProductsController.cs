using System.Threading.Tasks;
using MarketPlace.Application.Services.Interfaces;
using MarketPlace.DataLayer.DTOs.Common;
using MarketPlace.DataLayer.DTOs.Product;
using MarketPlace.Web.Http;
using Microsoft.AspNetCore.Mvc;

namespace MarketPlace.Web.Areas.Admin.Controllers
{
    public class ProductsController : AdminBaseController
    {
        #region Constructor

        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        #endregion

        #region Filter products

        public async Task<IActionResult> Index(FilterProductDto filter)
        {
            return View(await _productService.FilterProducts(filter));
        }

        #endregion

        #region Accept product

        public async Task<IActionResult> AcceptSellerProduct(long id)
        {
            var result = await _productService.AcceptSellerProduct(id);
            if (result)
            {
                return JsonResponseStatus.SendStatus(JsonResponseStatusType.Success, "محصول مورد نظر با موفقیت تایید شد", null);
            }

            return JsonResponseStatus.SendStatus(JsonResponseStatusType.Danger, "محصول مورد نظر یافت نشد", null);
        }

        #endregion

        #region Reject product

        public async Task<IActionResult> RejectProduct(RejectItemDto reject)
        {
            if (ModelState.IsValid)
            {
                var result = await _productService.RejectSellerProduct(reject);
                if (result)
                {
                    return JsonResponseStatus.SendStatus(JsonResponseStatusType.Success, "محصول با موفقیت رد شد", reject);
                }

                return JsonResponseStatus.SendStatus(JsonResponseStatusType.Success, "اطلاعات مورد نظر جهت عدم تایید را به درستی وارد نمایید", null);
            }

            return JsonResponseStatus.SendStatus(JsonResponseStatusType.Success, "محصول مورد نظر یافت نشد", null);
        }

        #endregion
    }
}
