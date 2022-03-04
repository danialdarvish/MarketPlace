using MarketPlace.DataLayer.DTOs.Account;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using MarketPlace.Application.Services.Interfaces;
using MarketPlace.Web.PresentationExtensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace MarketPlace.Web.Areas.User.Controllers
{
    public class AccountController : UserBaseController
    {
        #region Constructor

        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        #endregion

        #region Change password

        [HttpGet("change-password")]
        public async Task<IActionResult> ChangePassword()
        {
            return View();
        }

        [HttpPost("change-password"), ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto passwordDto)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.ChangeUserPassword(passwordDto, User.GetUserId());
                if (result)
                {
                    TempData[SuccessMessage] = "کلمه عبور شما تغییر یافت";
                    TempData[InfoMessage] = "لطفا جهت تکمیل فرآیند تغییر کلمه عبور، مجددا وارد سایت شوید";

                    await HttpContext.SignOutAsync();

                    return RedirectToAction("Login", "Account", new { area = "" });
                }
                else
                {
                    TempData[ErrorMessage] = "لطفا از کلمه عبور جدید استفاده کنید";
                    ModelState.AddModelError("NewPassword", "لطفا از کلمه عبور جدید استفاده کنید");
                }
            }

            return View(passwordDto);
        }

        #endregion

        #region Edit profile

        [HttpGet("edit-profile")]
        public async Task<IActionResult> EditProfile()
        {
            var userProfile = await _userService.GetProfileForEdit(User.GetUserId());
            if (userProfile == null) return NotFound();

            return View(userProfile);
        }

        [HttpPost("edit-profile"), ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(EditUserProfileDto profile, IFormFile avatarImage)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.EditUserProfile(profile, User.GetUserId(), avatarImage);
                switch (result)
                {
                    case EditUserProfileResult.IsBlocked:
                        TempData[ErrorMessage] = "حساب کاربری شما مسدود شده است";
                        break;
                    case EditUserProfileResult.IsNotActive:
                        TempData[ErrorMessage] = "حساب کاربری شما فعال نیست";
                        break;
                    case EditUserProfileResult.NotFound:
                        TempData[ErrorMessage] = "کاربری با مشخصات وارد شده یافت نشد";
                        break;
                    case EditUserProfileResult.Success:
                        TempData[SuccessMessage] = $"کاربر عزیز {profile.FirstName} {profile.LastName}، پروفایل شما با موفقیت ویرایش شد";
                        return RedirectToAction("EditProfile");
                }
            }

            return View(profile);
        }

        #endregion
    }
}
