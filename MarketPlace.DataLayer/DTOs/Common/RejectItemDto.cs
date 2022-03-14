using System.ComponentModel.DataAnnotations;

namespace MarketPlace.DataLayer.DTOs.Common
{
    public class RejectItemDto
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Display(Name="توضیحات عدم تایید اطلاعات")]
        public string RejectMessage { get; set; }
    }
}
