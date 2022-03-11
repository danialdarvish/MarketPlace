using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MarketPlace.DataLayer.DTOs.Paging;

namespace MarketPlace.DataLayer.DTOs.Seller
{
    public class FilterSellerDto : BasePaging
    {
        #region Properties

        public long? UserId { get; set; }
        public string StoreName { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public string Address { get; set; }
        public FilterSellerState State { get; set; }
        public List<Entities.Store.Seller> Sellers { get; set; }

        #endregion

        #region Methods

        public FilterSellerDto SetSellers(List<Entities.Store.Seller> sellers)
        {
            Sellers = sellers;
            return this;
        }

        public FilterSellerDto SetPaging(BasePaging paging)
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

    public enum FilterSellerState
    {
        [Display(Name = "همه")]
        All,
        [Display(Name = "در حال بررسی")]
        UnderProgress,

        [Display(Name = "تایید شده")]
        Accepted,

        [Display(Name = "رد شده")]
        Rejected
    }
}
