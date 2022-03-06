using System.Collections.Generic;
using MarketPlace.DataLayer.Entities.Contacts;

namespace MarketPlace.DataLayer.DTOs.Contacts
{
    public class TicketDetailDto
    {
        public Ticket Ticket { get; set; }
        public List<TicketMessage> TicketMessages { get; set; }

    }
}
