using System;

namespace MarketPlace.DataLayer.DTOs.Paging
{
    public class Pager
    {
        public static BasePaging Build(int pageId, int allEntitiesCount, int take, int showHowManyPagesBeforeAndAfter)
        {
            var pageCount = Convert.ToInt32(Math.Ceiling(allEntitiesCount / (double)take));
            return new BasePaging
            {
                PageId = pageId,
                AllEntitiesCount = allEntitiesCount,
                TakeEntity = take,
                SkipEntity = (pageId - 1) * take,
                StartPage = pageId - showHowManyPagesBeforeAndAfter <= 0 ? 1 : pageId - showHowManyPagesBeforeAndAfter,
                EndPage = pageId + showHowManyPagesBeforeAndAfter > pageCount ? pageCount : pageId + showHowManyPagesBeforeAndAfter,
                PageCount = pageCount,
                ShowHowManyPageAfterAndBefore = showHowManyPagesBeforeAndAfter
            };
        }
    }
}
