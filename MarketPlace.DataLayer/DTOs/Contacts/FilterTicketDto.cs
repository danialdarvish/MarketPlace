using System.Collections.Generic;
using MarketPlace.DataLayer.Entities.Contacts;

namespace MarketPlace.DataLayer.DTOs.Contacts
{
    public class FilterTicketDto
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
