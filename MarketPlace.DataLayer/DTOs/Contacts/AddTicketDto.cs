using System.ComponentModel.DataAnnotations;
using MarketPlace.DataLayer.Entities.Contacts;

namespace MarketPlace.DataLayer.DTOs.Contacts
{
    public class AddTicketDto
    {

        [Display(Name = "عنوان")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(200, ErrorMessage = "نمی توان بیشتر از {1} کاراکتر باشد {0}")]
        public string Title { get; set; }


        [Display(Name = "بخش مورد نظر")]
        public TicketSection TicketSection { get; set; }

        [Display(Name = "اولویت")]
        public TicketPriority TicketPriority { get; set; }

        [Display(Name = "متن پیام")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Text { get; set; }
    }

    public enum AddTicketResult
    {
        Error,
        Success
    }
}
