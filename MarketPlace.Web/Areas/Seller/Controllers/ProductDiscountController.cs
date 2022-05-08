using System.Threading.Tasks;
using MarketPlace.Application.Services.Interfaces;
using MarketPlace.DataLayer.DTOs.Product_Discount;
using MarketPlace.Web.PresentationExtensions;
using Microsoft.AspNetCore.Mvc;

namespace MarketPlace.Web.Areas.Seller.Controllers
{
    public class ProductDiscountController : SellerBaseController
    {
        #region Constructor

        private readonly ISellerService _sellerService;
        private readonly IProductDiscountService _productDiscountService;

        public ProductDiscountController(ISellerService sellerService, IProductDiscountService productDiscountService)
        {
            _sellerService = sellerService;
            _productDiscountService = productDiscountService;
        }

        #endregion

        #region Filter discount

        [HttpGet("discounts")]
        [HttpGet("discounts/{ProductId}")]
        public async Task<IActionResult> FilterDiscounts(FilterProductDiscountDto filter)
        {
            // if (filter.ProductId is null or 0) return NotFound();

            var seller = await _sellerService.GetLastActiveSellerByUserId(User.GetUserId());
            filter.SellerId = seller.Id;

            return View(await _productDiscountService.FilterProductDiscount(filter));
        }

        #endregion

        #region Create discount

        [HttpGet("create-discount")]
        public IActionResult CreateDiscount()
        {
            return View();
        }

        [HttpPost("create-discount"), ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDiscount(CreateProductDiscountDto discount)
        {
            if (ModelState.IsValid)
            {
                var seller = await _sellerService.GetLastActiveSellerByUserId(User.GetUserId());
                var result = await _productDiscountService.CreateProductDiscount(discount, seller.Id);

                switch (result)
                {
                    case CreateDiscountResult.Success:
                        TempData[SuccessMessage] = "تخفیف با موفقیت ثبت شد";
                        return RedirectToAction("FilterDiscounts", "ProductDiscount");
                    case CreateDiscountResult.ProductIsNotForSeller:
                        TempData[WarningMessage] = "محصول مورد نظر وجود ندارد";
                        break;
                    case CreateDiscountResult.ProductNotFound:
                        TempData[WarningMessage] = "محصول مورد نظر یافت نشد";
                        break;
                    case CreateDiscountResult.Error:
                        TempData[ErrorMessage] = "عملیات موفقیت آمیز نبود";
                        break;
                }
            }

            return View(discount);
        }

        #endregion
    }
}
