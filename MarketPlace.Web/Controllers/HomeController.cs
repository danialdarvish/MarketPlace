using GoogleReCaptcha.V3.Interface;
using MarketPlace.Application.Services.Interfaces;
using MarketPlace.DataLayer.DTOs.Contacts;
using MarketPlace.DataLayer.Entities.Site;
using MarketPlace.Web.PresentationExtensions;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MarketPlace.Web.Controllers
{
    public class HomeController : SiteBaseController
    {
        #region Constructor

        private readonly ISiteService _siteService;
        private readonly IContactService _contactService;
        private readonly ICaptchaValidator _captchaValidator;

        public HomeController(IContactService contactService, ICaptchaValidator captchaValidator, ISiteService siteService)
        {
            _siteService = siteService;
            _contactService = contactService;
            _captchaValidator = captchaValidator;
        }

        #endregion

        #region Index       

        public async Task<IActionResult> Index()
        {
            ViewBag.banners = await _siteService.GetSiteBannersByPlacement(new List<BannerPlacement>
            {
                BannerPlacement.Home_1,
                BannerPlacement.Home_2,
                BannerPlacement.Home_3
            });

            return View();
        }

        #endregion

        #region Contact us

        [HttpGet("contact-us")]
        public IActionResult ContactUs()
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

        #region About us

        [HttpGet("about-us")]
        public async Task<IActionResult> AboutUs()
        {
            var siteSetting = await _siteService.GetDefaultSiteSetting();
            return View(siteSetting);
        }

        #endregion
    }
}
