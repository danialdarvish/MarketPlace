using System.Collections.Generic;
using MarketPlace.DataLayer.DTOs.Paging;
using MarketPlace.DataLayer.Entities.Products;

namespace MarketPlace.DataLayer.DTOs.Product_Discount
{
    public class FilterProductDiscountDto : BasePaging
    {
        #region Properties

        public long? ProductId { get; set; }
        public long? SellerId { get; set; }
        public List<ProductDiscount> ProductDiscounts { get; set; }

        #endregion

        #region Methods

        public FilterProductDiscountDto SetDiscounts(List<ProductDiscount> productDiscounts)
        {
            ProductDiscounts = productDiscounts;
            return this;
        }

        public FilterProductDiscountDto SetPaging(BasePaging paging)
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
}
