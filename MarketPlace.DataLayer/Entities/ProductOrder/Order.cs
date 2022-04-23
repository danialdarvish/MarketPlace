using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MarketPlace.DataLayer.Entities.Common;

namespace MarketPlace.DataLayer.Entities.ProductOrder
{
    public class Order : BaseEntity
    {
        #region Properties

        public long UserId { get; set; }
        public DateTime? PaymentDate { get; set; }
        public bool IsPaid { get; set; }

        [Display(Name = "کد پیگیری")]
        [MaxLength(300, ErrorMessage = "نمی توان بیشتر از {1} کاراکتر باشد {0}")]
        public string TracingCode { get; set; }

        [Display(Name = "توضیحات")]
        public string Description { get; set; }

        #endregion

        #region Relations

        public ICollection<OrderDetail> OrderDetails { get; set; }

        #endregion
    }
}
