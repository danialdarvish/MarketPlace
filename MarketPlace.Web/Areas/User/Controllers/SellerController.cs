using System;
using System.Threading.Tasks;
using MarketPlace.Application.Services.Interfaces;
using MarketPlace.DataLayer.DTOs.Seller;
using MarketPlace.Web.PresentationExtensions;
using Microsoft.AspNetCore.Mvc;

namespace MarketPlace.Web.Areas.User.Controllers
{
    public class SellerController : UserBaseController
    {
        #region Constructor

        private readonly ISellerService _sellerService;

        public SellerController(ISellerService sellerService)
        {
            _sellerService = sellerService;
        }

        #endregion

        #region Request seller

        [HttpGet("request-seller-panel")]
        public IActionResult RequestSellerPanel()
        {
            return View();
        }

        [HttpPost("request-seller-panel"), ValidateAntiForgeryToken]
        public async Task<IActionResult> RequestSellerPanel(RequestSellerDto seller)
        {
            if (ModelState.IsValid)
            {
                var result = await _sellerService.AddNewSellerRequest(seller, User.GetUserId());

                switch (result)
                {
                    case RequestSellerResult.Success:
                        TempData[SuccessMessage] = "درخواست شما با موفقیت ثبت شد";
                        TempData[InfoMessage] = "فرایند تایید اطلاعات شما در حال پیگیری می باشد";
                        return RedirectToAction("SellerRequests");
                    case RequestSellerResult.HasUnderProgressRequest:
                        TempData[WarningMessage] = "درخواست های قبلی شما در حال پیگیری می باشند";
                        TempData[InfoMessage] = "در حال حاضر نمی توانید درخواست جدیدی ثبت کنید";
                        break;
                    case RequestSellerResult.HasNotPermission:
                        TempData[ErrorMessage] = "شما دسترسی لازم جهت انجام فرایند مورد نظر را ندارید";
                        break;
                }
            }


            return View();
        }

        #endregion

        #region Seller requests

        [HttpGet("seller-requests")]
        public async Task<IActionResult> SellerRequests()
        {
            return View();
        }

        #endregion
    }
}
