using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketPlace.Web.Areas.Seller.Controllers
{
    [Authorize]
    [Area("Seller")]
    [Route("seller")]
    public class SellerBaseController : Controller
    {
        protected string SuccessMessage = nameof(SuccessMessage);
        protected string ErrorMessage = nameof(ErrorMessage);
        protected string WarningMessage = nameof(WarningMessage);
        protected string InfoMessage = nameof(InfoMessage);
    }
}
