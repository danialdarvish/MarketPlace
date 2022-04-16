using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MarketPlace.DataLayer.DTOs.Paging;

namespace MarketPlace.DataLayer.DTOs.Product
{
    public class FilterProductDto : BasePaging
    {
        #region Constructor

        public FilterProductDto()
        {
            FilterProductOrderBy = FilterProductOrderBy.CreateDate_Des;
        }

        #endregion

        #region Properties

        public string Title { get; set; }
        public string Category { get; set; }
        public long? SellerId { get; set; }
        public int FilterMinPrice { get; set; }
        public int FilterMaxPrice { get; set; }
        public int SelectedMinPrice { get; set; }
        public int SelectedMaxPrice { get; set; }
        public int PriceStep { get; set; } = 100000;
        public List<Entities.Products.Product> Products { get; set; }
        public FilterProductState FilterProductState { get; set; }
        public FilterProductOrderBy FilterProductOrderBy { get; set; }
        public List<long> SelectedProductCategories { get; set; }

        #endregion

        #region Methods

        public FilterProductDto SetProducts(List<Entities.Products.Product> products)
        {
            Products = products;
            return this;
        }

        public FilterProductDto SetPaging(BasePaging paging)
        {
            PageId = paging.PageId;
            AllEntitiesCount = paging.AllEntitiesCount;
            StartPage = paging.StartPage;
            EndPage = paging.EndPage;
            ShowHowManyPageAfterAndBefore = paging.ShowHowManyPageAfterAndBefore;
            TakeEntity = paging.TakeEntity;
            SkipEntity = paging.SkipEntity;
            PageCount = paging.PageCount;

            return this;
        }

        #endregion
    }

    public enum FilterProductState
    {
        [Display(Name = "همه")]
        All,

        [Display(Name = "در حال بررسی")]
        UnderProgress,

        [Display(Name = "تایید شده")]
        Accepted,

        [Display(Name = "رد شده")]
        Rejected,

        [Display(Name = "فعال")]
        Active,

        [Display(Name = "غیرفعال")]
        NotActive
    }

    public enum FilterProductOrderBy
    {
        [Display(Name = "تاریخ(نزولی)")]
        CreateDate_Des,

        [Display(Name = "تاریخ(صعودی)")]
        CreateDate_Asc,

        [Display(Name = "قیمت(نزولی)")]
        Price_Des,

        [Display(Name = "قیمت(صعودی)")]
        Price_Asc,
    }
}
