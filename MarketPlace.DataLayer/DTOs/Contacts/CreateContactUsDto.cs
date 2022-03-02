using MarketPlace.DataLayer.DTOs.Site;
using System.ComponentModel.DataAnnotations;

namespace MarketPlace.DataLayer.DTOs.Contacts
{
    public class CreateContactUsDto : CaptchaViewModel
    {
        [Display(Name = "ایمیل")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(200, ErrorMessage = "نمی توان بیشتر از {1} کاراکتر باشد {0}")]
        public string Email { get; set; }

        [Display(Name = "نام و نام خانوادگی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(200, ErrorMessage = "نمی توان بیشتر از {1} کاراکتر باشد {0}")]
        public string FullName { get; set; }

        [Display(Name = "عنوان")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(200, ErrorMessage = "نمی توان بیشتر از {1} کاراکتر باشد {0}")]
        public string Subject { get; set; }

        [Display(Name = "متن پیام")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Text { get; set; }
    }
}
