using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketPlace.Web.Areas.Admin.Controllers
{

    [Authorize]
    [Area("Admin")]
    //[Route("admin")]
    public class AdminBaseController : Controller
    {
        protected string SuccessMessage = nameof(SuccessMessage);
        protected string ErrorMessage = nameof(ErrorMessage);
        protected string WarningMessage = nameof(WarningMessage);
        protected string InfoMessage = nameof(InfoMessage);
    }
}
