using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MarketPlace.Web.ViewComponents
{
    #region SiteHeader
    public class SiteHeaderViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View("SiteHeader");
        }
    }
    #endregion

    #region SiteFooter
    public class SiteFooterViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View("SiteFooter");
        }
    }
    #endregion
}
