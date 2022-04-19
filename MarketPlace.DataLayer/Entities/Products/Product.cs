using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MarketPlace.DataLayer.Entities.Common;
using MarketPlace.DataLayer.Entities.Store;

namespace MarketPlace.DataLayer.Entities.Products
{
    public class Product : BaseEntity
    {
        #region Properties

        public long SellerId { get; set; }

        [Display(Name = "نام محصول")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(300, ErrorMessage = "نمی توان بیشتر از {1} کاراکتر باشد {0}")]
        public string Title { get; set; }

        [Display(Name = "تصویر محصول")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(300, ErrorMessage = "نمی توان بیشتر از {1} کاراکتر باشد {0}")]
        public string ImageName { get; set; }

        [Display(Name = "قیمت محصول")]
        public int Price { get; set; }

        [Display(Name = "توضیحات کوتاه")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(500, ErrorMessage = "نمی توان بیشتر از {1} کاراکتر باشد {0}")]
        public string ShortDescription { get; set; }

        [Display(Name = "توضیحات اصلی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Description { get; set; }

        [Display(Name = "پیام تایید / عدم تایید محصول")]
        public string ProductAcceptOrRejectDescription { get; set; }

        [Display(Name = "فعال / غیر فعال بودن")]
        public bool IsActive { get; set; }

        [Display(Name = "وضعیت")]
        public ProductAcceptanceState ProductAcceptanceState { get; set; }

        #endregion

        #region Relations

        public ICollection<ProductSelectedCategory> ProductSelectedCategories { get; set; }
        public ICollection<ProductColor> ProductColors { get; set; }
        public ICollection<ProductGallery> ProductGalleries { get; set; }
        public Seller Seller { get; set; }
        public ICollection<ProductFeature> ProductFeatures { get; set; }

        #endregion
    }

    public enum ProductAcceptanceState
    {
        [Display(Name = "در حال بررسی")]
        UnderProgress,

        [Display(Name = "تایید شده")]
        Accepted,

        [Display(Name = "رد شده")]
        Rejected
    }
}
