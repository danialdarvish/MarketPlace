using System.Threading.Tasks;
using MarketPlace.Application.Services.Interfaces;
using MarketPlace.Application.Utils;
using MarketPlace.DataLayer.DTOs.Common;
using MarketPlace.DataLayer.DTOs.Orders;
using MarketPlace.Web.Http;
using MarketPlace.Web.PresentationExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketPlace.Web.Areas.User.Controllers
{
    public class OrderController : UserBaseController
    {
        #region Constructor

        private readonly IUserService _userService;
        private readonly IOrderService _orderService;
        private readonly IPaymentService _paymentService;

        public OrderController(IUserService userService, IOrderService orderService, IPaymentService paymentService)
        {
            _userService = userService;
            _orderService = orderService;
            _paymentService = paymentService;
        }

        #endregion

        #region Add product to open order

        [AllowAnonymous]
        [HttpPost("add-product-to-order")]
        public async Task<IActionResult> AddProductToOrder(AddProductToOrderDto order)
        {
            if (ModelState.IsValid)
            {
                if (User.Identity.IsAuthenticated)
                {
                    await _orderService.AddProductToOpenOrder(order, User.GetUserId());
                    return JsonResponseStatus
                        .SendStatus(JsonResponseStatusType.Success, "محصول مورد نظر با موفقیت ثبت شد", null);
                }
                else
                {
                    return JsonResponseStatus
                        .SendStatus(JsonResponseStatusType.Danger, "برای ثبت محصول در سبد خرید ابتدا باید وارد سایت شوید", null);
                }
            }

            return JsonResponseStatus
                .SendStatus(JsonResponseStatusType.Danger, "در ثبت اطلاعات خطایی رخ داد", null);
        }

        #endregion

        #region Open order

        [HttpGet("open-order")]
        public async Task<IActionResult> UserOpenOrder()
        {
            //var openOrder = await _orderService.GetUserLatestOpenOrder(User.GetUserId());
            var openOrder = await _orderService.GetUserOpenOrderDetail(User.GetUserId());
            return View(openOrder);
        }

        #endregion

        #region Pay order

        [HttpGet("pay-order")]
        public async Task<IActionResult> PayUserOrderPrice()
        {
            var openOrderAmount = await _orderService.GetTotalOrderPriceForPayment(User.GetUserId());
            string callBackUrl = PathExtension.DomainAddress + Url.RouteUrl("ZarinpalPaymentResult");
            string redirectUrl = "";

            var status = _paymentService.CreatePaymentRequest(
                null,
                openOrderAmount,
                "تکمیل فرایند خرید از سایت",
                callBackUrl, ref redirectUrl);

            if (status == PaymentStatus.St100) return Redirect(redirectUrl);

            return RedirectToAction("UserOpenOrder");
        }

        #endregion

        #region Callback zarinpal

        [AllowAnonymous]
        [HttpGet("payment-result", Name = "ZarinpalPaymentResult")]
        public async Task<IActionResult> CallBackZarinpal()
        {
            string authority = _paymentService.GetAuthorityCodeFromCallback(HttpContext);
            if (authority == "")
            {
                TempData[WarningMessage] = "عملیات پرداخت با شکست مواجه شد";
                return View();
            }

            var openOrderAmount = await _orderService.GetTotalOrderPriceForPayment(User.GetUserId());
            long refId = 0;
            var res = _paymentService
                .PaymentVerification(null, authority, openOrderAmount, ref refId);
            if (res == PaymentStatus.St100)
            {
                TempData[SuccessMessage] = "پرداخت شما با موفقیت انجام شد";
                TempData[InfoMessage] = "کد پیگیری شما: " + refId;

                await _orderService
                    .PayOrderProductPriceToSeller(User.GetUserId(), refId);

                return View();
            }
            TempData[WarningMessage] = "عملیات پرداخت با خطا مواجه شد";

            return View();
        }

        #endregion

        #region Open order partial

        [HttpGet("change-detail-count/{detailId}/{count}")]
        public async Task<IActionResult> ChangeDetailCount(long detailId, int count)
        {
            await Task.Delay(500); // Should not be in publish
            await _orderService.ChangeOrderDetailCount(detailId, User.GetUserId(), count);
            var openOrder = await _orderService.GetUserOpenOrderDetail(User.GetUserId());
            return PartialView(openOrder);
        }

        #endregion

        #region Remove product from order

        [HttpGet("remove-order-item/{detailId}")]
        public async Task<IActionResult> RemoveProductFromOrder(long detailId)
        {
            var result = await _orderService.RemoveOrderDetail(detailId, User.GetUserId());
            if (result)
            {
                TempData[SuccessMessage] = "محصول مورد نظر با موفقیت از سبد خرید حذف شد";
                return JsonResponseStatus.SendStatus(JsonResponseStatusType.Success,
                    "محصول مورد نظر با موفقیت از سبد خرید حذف شد", null);
            }

            TempData[ErrorMessage] = "محصول مورد نظر در سبد خرید شما یافت نشد";
            return JsonResponseStatus.SendStatus(JsonResponseStatusType.Danger,
                "محصول مورد نظر در سبد خرید شما یافت نشد", null);
        }

        #endregion
    }
}
