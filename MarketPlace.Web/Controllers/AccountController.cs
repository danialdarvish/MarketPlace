using MarketPlace.Application.Services.Interfaces;
using MarketPlace.DataLayer.DTOs.Account;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MarketPlace.Web.Controllers
{
    public class AccountController : SiteBaseController
    {
        #region Constructor
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }
        #endregion

        #region Register
        [HttpGet("register")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost("register"), ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterUserDto register)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.RegisterUser(register);
                switch (result)
                {
                    case RegisterUserResult.MobileExists:
                        TempData["ErrorMessage"] = "تلفن همراه وارد شده تکراری می باشد";
                        ModelState.AddModelError("mobile", "تلفن همراه وارد شده تکراری می باشد");
                        break;
                    case RegisterUserResult.Success:
                        TempData["SuccessMessage"] = "ثبت نام شما با موفقیت انجام شد";
                        TempData["InfoMessage"] = "کد تایید تلفن همراه برای شما ارسال شد";
                        return RedirectToAction("Login");
                }
            }

            return View(register);
        }
        #endregion

        #region Login
        public IActionResult Login()
        {
            return View();
        }
        #endregion
    }
}
