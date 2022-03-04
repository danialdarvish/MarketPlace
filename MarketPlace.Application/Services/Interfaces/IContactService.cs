using MarketPlace.DataLayer.DTOs.Contacts;
using System;
using System.Threading.Tasks;

namespace MarketPlace.Application.Services.Interfaces
{
    public interface IContactService : IAsyncDisposable
    {
        #region Ticket

        Task<AddTicketResult> AddUserTicket(AddTicketViewModel ticket, long userId);
        Task<FilterTicketDto> FilterTickets(FilterTicketDto filter);

        #endregion

        #region Contact us
        Task CreateContactUs(CreateContactUsDto contact, string userIp, long? userId);

        #endregion
    }
}
