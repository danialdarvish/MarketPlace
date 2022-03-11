using System.ComponentModel.DataAnnotations;

namespace MarketPlace.DataLayer.DTOs.Seller
{
    public class EditRequestSellerDto
    {
        public long Id { get; set; }
        
        [Display(Name = "نام فروشگاه")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(200, ErrorMessage = "نمی توان بیشتر از {1} کاراکتر باشد {0}")]
        public string StoreName { get; set; }

        [Display(Name = "تلفن فروشگاه")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(50, ErrorMessage = "نمی توان بیشتر از {1} کاراکتر باشد {0}")]
        public string Phone { get; set; }

        [Display(Name = "آدرس")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(500, ErrorMessage = "نمی توان بیشتر از {1} کاراکتر باشد {0}")]
        public string Address { get; set; }
    }

    public enum EditRequestSellerResult
    {
        NotFound,
        Success
    }
}
