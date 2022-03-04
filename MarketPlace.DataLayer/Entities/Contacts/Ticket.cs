using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MarketPlace.DataLayer.Entities.Account;
using MarketPlace.DataLayer.Entities.Common;

namespace MarketPlace.DataLayer.Entities.Contacts
{
    public class Ticket : BaseEntity
    {

        #region Properties

        public long OwnerId { get; set; }

        [Display(Name = "عنوان")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(200, ErrorMessage = "نمی توان بیشتر از {1} کاراکتر باشد {0}")]
        public string Title { get; set; }

        [Display(Name = "وضعیت تیکت")]
        public TicketState TicketState { get; set; }

        [Display(Name = "بخش مورد نظر")]
        public TicketSection TicketSection { get; set; }

        [Display(Name = "اولویت")]
        public TicketPriority TicketPriority { get; set; }

        public bool IsReadByOwner { get; set; }
        public bool IsReadByAdmin { get; set; }

        #endregion

        #region Relations

        public User Owner { get; set; }
        public ICollection<TicketMessage> TicketMessages { get; set; }

        #endregion
    }

    public enum TicketSection
    {
        [Display(Name = "پشتیبانی")]
        Support,

        [Display(Name = "فنی")]
        Technical,

        [Display(Name = "آموزشی")]
        Academic
    }

    public enum TicketPriority
    {
        [Display(Name = "کم")]
        Low,

        [Display(Name = "متوسط")]
        Medium,

        [Display(Name = "زیاد")]
        High
    }

    public enum TicketState
    {
        [Display(Name = "در حال بررسی")]
        UnderProgress,

        [Display(Name = "پاسخ داده شده")]
        Answered,

        [Display(Name = "بسته شده")]
        Closed

    }
}
