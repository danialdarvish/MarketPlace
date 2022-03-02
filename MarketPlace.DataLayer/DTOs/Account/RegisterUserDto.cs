using MarketPlace.DataLayer.DTOs.Site;
using System.ComponentModel.DataAnnotations;

namespace MarketPlace.DataLayer.DTOs.Account
{
    public class RegisterUserDto : CaptchaViewModel
    {
        [Display(Name = "تلفن همراه")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(200, ErrorMessage = "نمی توان بیشتر از {1} کاراکتر باشد {0}")]
        public string Mobile { get; set; }

        [Display(Name = "نام")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(200, ErrorMessage = "نمی توان بیشتر از {1} کاراکتر باشد {0}")]
        public string FirstName { get; set; }

        [Display(Name = "نام خانوادگی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(200, ErrorMessage = "نمی توان بیشتر از {1} کاراکتر باشد {0}")]
        public string LastName { get; set; }

        [Display(Name = "کلمه عبور")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(200, ErrorMessage = "نمی توان بیشتر از {1} کاراکتر باشد {0}")]
        public string Password { get; set; }

        [Display(Name = "تکرار کلمه عبور")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(200, ErrorMessage = "نمی توان بیشتر از {1} کاراکتر باشد {0}")]
        [Compare("Password", ErrorMessage = "کلمه های عبور مغایرت دارند.")]
        public string ConfirmPassword { get; set; }
    }

    public enum RegisterUserResult
    {
        Success,
        MobileExists
    }
}
