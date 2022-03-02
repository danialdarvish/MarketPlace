using GoogleReCaptcha.V3.Interface;
using MarketPlace.Application.Services.Interfaces;
using MarketPlace.DataLayer.DTOs.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MarketPlace.Web.Controllers
{
    public class AccountController : SiteBaseController
    {
        #region Constructor
        private readonly IUserService _userService;
        private readonly ICaptchaValidator _captchaValidator;
        public AccountController(IUserService userService, ICaptchaValidator captchaValidator)
        {
            _userService = userService;
            _captchaValidator = captchaValidator;
        }
        #endregion

        #region Register
        [HttpGet("register")]
        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated) return Redirect("/");
            return View();
        }

        [HttpPost("register"), ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterUserDto register)
        {
            if (!await _captchaValidator.IsCaptchaPassedAsync(register.Captcha))
            {
                TempData[ErrorMessage] = "کد تصویر شما با تصویر مطابقت ندارد";
                return View(register);
            }

            if (ModelState.IsValid)
            {
                var result = await _userService.RegisterUser(register);
                switch (result)
                {
                    case RegisterUserResult.MobileExists:
                        TempData[ErrorMessage] = "تلفن همراه وارد شده تکراری می باشد";
                        ModelState.AddModelError("mobile", "تلفن همراه وارد شده تکراری می باشد");
                        break;
                    case RegisterUserResult.Success:
                        TempData[SuccessMessage] = "ثبت نام شما با موفقیت انجام شد";
                        TempData[InfoMessage] = "کد تایید تلفن همراه برای شما ارسال شد";
                        return RedirectToAction("Login");
                }
            }

            return View(register);
        }
        #endregion

        #region Login
        [HttpGet("login")]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated) return Redirect("/");
            return View();
        }

        [HttpPost("login"), ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginUserDto login)
        {
            if (!await _captchaValidator.IsCaptchaPassedAsync(login.Captcha))
            {
                TempData[ErrorMessage] = "کد تصویر شما با تصویر مطابقت ندارد";
                return View(login);
            }

            if (ModelState.IsValid)
            {
                var result = await _userService.GetUserForLogin(login);
                switch (result)
                {
                    case LoginUserResult.NotFound:
                        TempData[ErrorMessage] = "کاربر مورد نظر یافت نشد";
                        break;
                    case LoginUserResult.NotActivated:
                        TempData[WarningMessage] = "حساب کاربری شما فعال نشده است";
                        break;
                    case LoginUserResult.Success:
                        var user = await _userService.GetUserByMobile(login.Mobile);
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, user.Mobile),
                            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                        };
                        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var principal = new ClaimsPrincipal(identity);
                        var properties = new AuthenticationProperties
                        {
                            IsPersistent = login.RememberMe
                        };

                        await HttpContext.SignInAsync(principal, properties);

                        TempData[SuccessMessage] = "عملیات ورود با موفقیت انجام شد";
                        return Redirect("/");
                }
            }


            return View(login);
        }
        #endregion

        #region Logout

        [HttpGet("log-out")]
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }
        #endregion

        #region Forgot Password

        [HttpGet("forgot-pass")]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost("forgot-pass"), ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto forgot)
        {
            if (!await _captchaValidator.IsCaptchaPassedAsync(forgot.Captcha))
            {
                TempData[ErrorMessage] = "کد تصویر شما با تصویر مطابقت ندارد";
                return View(forgot);
            }

            if (ModelState.IsValid)
            {
                var result = await _userService.RecoverUserPassword(forgot);
                switch (result)
                {
                    case ForgotPasswordResult.NotFound:
                        TempData[WarningMessage] = "کاربر یافت نشد";
                        break;
                    case ForgotPasswordResult.Success:
                        TempData[SuccessMessage] = "کلمه عبور جدید برای شما ارسال شد";
                        TempData[InfoMessage] = "لطفا پس از ورود به حساب کاربری، کلمه عبور خود را تغییر دهید";
                        return RedirectToAction("Login");
                }
            }

            return View(forgot);
        }
        #endregion
    }
}
