using System.Collections.Generic;
using System.Reflection.Metadata;
using MarketPlace.DataLayer.DTOs.Paging;
using MarketPlace.DataLayer.Entities.Contacts;

namespace MarketPlace.DataLayer.DTOs.Contacts
{
    public class FilterTicketDto : BasePaging
    {
        #region Properties

        public string Title { get; set; }
        public long? UserId { get; set; }
        public TicketSection? TicketSection { get; set; }
        public TicketPriority? TicketPriority { get; set; }
        public FilterTicketState FilterTicketState { get; set; }
        public FilterTicketOrder OrderBy { get; set; }
        public List<Ticket> Tickets { get; set; }

        #endregion

        #region Methods

        public FilterTicketDto SetTickets(List<Ticket> tickets)
        {
            Tickets = tickets;
            return this;
        }

        public FilterTicketDto SetPaging(BasePaging paging)
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

    public enum FilterTicketState
    {
        All,
        NotDeleted,
        Deleted
    }

    public enum FilterTicketOrder
    {
        CreateDate_DES,
        CreateDate_ASC
    }
}
