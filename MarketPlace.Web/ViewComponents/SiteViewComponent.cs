using MarketPlace.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MarketPlace.Web.ViewComponents
{
    #region SiteHeader
    public class SiteHeaderViewComponent : ViewComponent
    {
        private readonly ISiteService _siteService;

        public SiteHeaderViewComponent(ISiteService siteService)
        {
            _siteService = siteService;
        }


        public async Task<IViewComponentResult> InvokeAsync()
        {
            ViewBag.siteSetting = await _siteService.GetDefaultSiteSetting();
            return View("SiteHeader");
        }
    }
    #endregion

    #region SiteFooter
    public class SiteFooterViewComponent : ViewComponent
    {
        private readonly ISiteService _siteService;

        public SiteFooterViewComponent(ISiteService siteService)
        {
            _siteService = siteService;
        }


        public async Task<IViewComponentResult> InvokeAsync()
        {
            ViewBag.siteSetting = await _siteService.GetDefaultSiteSetting();
            return View("SiteFooter");
        }
    }
    #endregion
}
