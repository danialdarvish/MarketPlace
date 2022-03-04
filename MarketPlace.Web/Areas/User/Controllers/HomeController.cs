using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MarketPlace.Web.Areas.User.Controllers
{
    public class HomeController : UserBaseController
    {
        #region Constructor



        #endregion

        #region User Dashboard

        [HttpGet("")]
        public async Task<IActionResult> Dashboard()
        {
            return View();
        }

        #endregion
    }
}
