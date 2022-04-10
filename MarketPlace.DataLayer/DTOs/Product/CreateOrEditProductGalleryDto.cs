using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace MarketPlace.DataLayer.DTOs.Product
{
    public class CreateOrEditProductGalleryDto
    {
        [Display(Name="اولویت نمایش")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public int DisplayPriority { get; set; }

        [Display(Name = "تصویر")]
        public IFormFile Image { get; set; }

        public string ImageName { get; set; }
    }

    public enum CreateOrEditProductGalleryResult
    {
        Success,
        NotForUserProduct,
        ImageIsNull,
        GalleryNotFound
    }
}
