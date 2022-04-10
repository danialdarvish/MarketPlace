using System.ComponentModel.DataAnnotations;
using MarketPlace.DataLayer.Entities.Common;

namespace MarketPlace.DataLayer.Entities.Products
{
    public class ProductGallery : BaseEntity
    {
        #region Properties

        public long ProductId { get; set; }

        [Display(Name = "اولویت تصوير")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public int DisplayPriority { get; set; }

        [Display(Name = "نام تصوير")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(300, ErrorMessage = "نمی توان بیشتر از {1} کاراکتر باشد {0}")]
        public string ImageName { get; set; }

        #endregion

        #region Relations

        public Product Product { get; set; }

        #endregion
    }
}
