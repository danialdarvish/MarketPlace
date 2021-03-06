using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MarketPlace.DataLayer.Entities.Products;

namespace MarketPlace.DataLayer.DTOs.Product
{
    public class ProductDetailDto
    {
        public long ProductId { get; set; }
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

        public Entities.Store.Seller Seller { get; set; }
        public List<ProductGallery> ProductGalleries { get; set; }
        public List<ProductColor> ProductColors { get; set; }
        public List<ProductCategory> ProductCategories { get; set; }
        public List<ProductFeature> ProductFeatures { get; set; }
        public List<Entities.Products.Product> RelatedProducts { get; set; }
    }
}
