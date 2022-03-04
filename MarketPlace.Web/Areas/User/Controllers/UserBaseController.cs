using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketPlace.Web.Areas.User.Controllers
{
    [Authorize]
    [Area("User")]
    [Route("user")]
    public class UserBaseController : Controller
    {
        protected string SuccessMessage = nameof(SuccessMessage);
        protected string ErrorMessage = nameof(ErrorMessage);
        protected string WarningMessage = nameof(WarningMessage);
        protected string InfoMessage = nameof(InfoMessage);
    }
}
