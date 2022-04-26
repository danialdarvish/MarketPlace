using System.Threading.Tasks;
using MarketPlace.Application.Services.Interfaces;
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

        public OrderController(IUserService userService, IOrderService orderService)
        {
            _userService = userService;
            _orderService = orderService;
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

        #region Open cart

        [HttpGet("open-order")]
        public async Task<IActionResult> UserOpenOrder()
        {
            //var openOrder = await _orderService.GetUserLatestOpenOrder(User.GetUserId());
            var openOrder = await _orderService.GetUserOpenOrderDetail(User.GetUserId());
            return View(openOrder);
        }

        #endregion
    }
}
