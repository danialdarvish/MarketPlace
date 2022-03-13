using System.Threading.Tasks;
using MarketPlace.Application.Services.Interfaces;
using MarketPlace.DataLayer.DTOs.Seller;
using MarketPlace.Web.Http;
using Microsoft.AspNetCore.Mvc;

namespace MarketPlace.Web.Areas.Admin.Controllers
{
    public class SellerController : AdminBaseController
    {
        #region Constructor

        private readonly ISellerService _sellerService;

        public SellerController(ISellerService sellerService)
        {
            _sellerService = sellerService;
        }

        #endregion

        #region Seller requests

        public async Task<IActionResult> SellerRequests(FilterSellerDto filter)
        {
            return View(await _sellerService.FilterSellers(filter));
        }

        #endregion

        #region Accept seller request

        public async Task<IActionResult> AcceptSellerRequest(long requestId)
        {
            var result = await _sellerService.AcceptSellerRequest(requestId);

            if (result) return JsonResponseStatus.SendStatus(JsonResponseStatusType.Success,
                    "درخواست مورد نظر با موفقیت تایید شد", null);

            return JsonResponseStatus.SendStatus(JsonResponseStatusType.Danger,
                "اطلاعاتی با این مشخصات یافت نشد", null);
        }

        #endregion
    }
}
