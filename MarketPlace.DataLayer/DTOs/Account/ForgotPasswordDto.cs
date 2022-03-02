using MarketPlace.DataLayer.DTOs.Site;
using System.ComponentModel.DataAnnotations;

namespace MarketPlace.DataLayer.DTOs.Account
{
    public class ForgotPasswordDto : CaptchaViewModel
    {
        [Display(Name = "تلفن همراه")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(200, ErrorMessage = "تعداد کاراکتر مجاز نیست")]
        public string Mobile { get; set; }
    }

    public enum ForgotPasswordResult
    {
        Success,
        NotFound,
        Error
    }
}
