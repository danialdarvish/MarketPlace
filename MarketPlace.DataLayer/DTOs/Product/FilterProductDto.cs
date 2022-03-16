﻿using System.Collections.Generic;
using MarketPlace.DataLayer.DTOs.Paging;

namespace MarketPlace.DataLayer.DTOs.Product
{
    public class FilterProductDto : BasePaging
    {
        #region Properties

        public string Title { get; set; }
        public long? SellerId { get; set; }
        public List<Entities.Products.Product> Products { get; set; }
        public FilterProductState FilterProductState { get; set; }

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
        UnderProgress,
        Accepted,
        Rejected,
        Active,
        NotActive
    }
}