using MarketPlace.DataLayer.DTOs.Contacts;
using System;
using System.Threading.Tasks;

namespace MarketPlace.Application.Services.Interfaces
{
    public interface IContactService : IAsyncDisposable
    {
        #region Ticket

        Task<AddTicketResult> AddUserTicket(AddTicketDto ticket, long userId);
        Task<FilterTicketDto> FilterTickets(FilterTicketDto filter);
        Task<TicketDetailDto> GetTicketForShow(long ticketId, long userId);

        #endregion

        #region Contact us
        Task CreateContactUs(CreateContactUsDto contact, string userIp, long? userId);

        #endregion
    }
}
