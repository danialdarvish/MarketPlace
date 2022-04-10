using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using MarketPlace.Application.Services.Interfaces;
using MarketPlace.Web.PresentationExtensions;

namespace MarketPlace.Web.Areas.User.ViewComponents
{
    public class UserSidebarViewComponent : ViewComponent
    {
        private ISellerService _sellerService;

        public UserSidebarViewComponent(ISellerService sellerService)
        {
            _sellerService = sellerService;
        }


        public async Task<IViewComponentResult> InvokeAsync()
        {
            ViewBag.hasUserAnyActiveSeller = await _sellerService.HasUserAnyActiveSellerPanel(User.GetUserId());
            return View("UserSidebar");
        }
    }
}
