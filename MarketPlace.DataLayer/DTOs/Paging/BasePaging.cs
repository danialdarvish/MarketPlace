using System;

namespace MarketPlace.DataLayer.DTOs.Paging
{
    public class BasePaging
    {
        public int PageId { get; set; }
        public int PageCount { get; set; }
        public int AllEntitiesCount { get; set; }
        public int StartPage { get; set; }
        public int EndPage { get; set; }
        public int TakeEntity { get; set; }
        public int SkipEntity { get; set; }
        public int ShowHowManyPageAfterAndBefore { get; set; }

        public BasePaging()
        {
            PageId = 1;
            TakeEntity = 10;
            ShowHowManyPageAfterAndBefore = 3;
        }
    }
}
