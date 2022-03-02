using GoogleReCaptcha.V3.Interface;
using MarketPlace.Application.Services.Interfaces;
using MarketPlace.DataLayer.DTOs.Contacts;
using MarketPlace.Web.PresentationExtensions;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MarketPlace.Web.Controllers
{
    public class HomeController : SiteBaseController
    {
        #region Constructor

        private readonly IContactService _contactService;
        private readonly ICaptchaValidator _captchaValidator;

        public HomeController(IContactService contactService, ICaptchaValidator captchaValidator)
        {
            _contactService = contactService;
            _captchaValidator = captchaValidator;
        }

        #endregion

        #region Contact Us

        [HttpGet("contact-us")]
        public async Task<IActionResult> ContactUs()
        {
            return View();
        }

        [HttpPost("contact-us"), ValidateAntiForgeryToken]
        public async Task<IActionResult> ContactUs(CreateContactUsDto contact)
        {
            if (!await _captchaValidator.IsCaptchaPassedAsync(contact.Captcha))
            {
                TempData[ErrorMessage] = "کد تصویر شما با تصویر مطابقت ندارد";
                return View(contact);
            }
            if (ModelState.IsValid)
            {
                await _contactService
                    .CreateContactUs(contact, HttpContext.GetUserIp(), User.GetUserId());

                TempData[SuccessMessage] = "پیام شما با موفقیت ارسال شد";
                RedirectToAction("ContactUs");
            }

            return View(contact);
        }
        #endregion

        #region Index       

        public IActionResult Index()
        {
            return View();
        }

        #endregion
    }
}
