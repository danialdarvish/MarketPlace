using System.ComponentModel.DataAnnotations;
using System.Linq;
using MarketPlace.Application.Services.Interfaces;
using MarketPlace.DataLayer.DTOs.Contacts;
using MarketPlace.DataLayer.Entities.Contacts;
using MarketPlace.DataLayer.Repository;
using System.Threading.Tasks;
using MarketPlace.DataLayer.DTOs.Paging;
using Microsoft.EntityFrameworkCore;

namespace MarketPlace.Application.Services.Implementations
{
    public class ContactService : IContactService
    {
        #region Constructor

        private readonly IGenericRepository<Ticket> _ticketRepository;
        private readonly IGenericRepository<ContactUs> _contactUsRepository;
        private readonly IGenericRepository<TicketMessage> _ticketMessageRepository;

        public ContactService(IGenericRepository<Ticket> ticketRepository, IGenericRepository<ContactUs> contactUsRepository, IGenericRepository<TicketMessage> ticketMessageRepository)
        {
            _ticketRepository = ticketRepository;
            _contactUsRepository = contactUsRepository;
            _ticketMessageRepository = ticketMessageRepository;
        }

        #endregion

        #region Ticket

        public async Task<AddTicketResult> AddUserTicket(AddTicketDto ticket, long userId)
        {
            if (string.IsNullOrEmpty(ticket.Text)) return AddTicketResult.Error;

            // Add ticket
            var newTicket = new Ticket
            {
                OwnerId = userId,
                IsReadByOwner = true,
                TicketPriority = ticket.TicketPriority,
                Title = ticket.Title,
                TicketSection = ticket.TicketSection,
                TicketState = TicketState.UnderProgress
            };

            await _ticketRepository.AddEntity(newTicket);
            await _ticketRepository.SaveChanges();

            // Add ticket message
            var newMessage = new TicketMessage
            {
                TicketId = newTicket.Id,
                Text = ticket.Text,
                SenderId = userId
            };

            await _ticketMessageRepository.AddEntity(newMessage);
            await _ticketMessageRepository.SaveChanges();

            return AddTicketResult.Success;
        }

        public async Task<FilterTicketDto> FilterTickets(FilterTicketDto filter)
        {
            var query = _ticketRepository.GetQuery();

            #region State

            switch (filter.FilterTicketState)
            {
                case FilterTicketState.All:
                    break;
                case FilterTicketState.Deleted:
                    query = query.Where(x => x.IsDelete);
                    break;
                case FilterTicketState.NotDeleted:
                    query = query.Where(x => !x.IsDelete);
                    break;
            }

            switch (filter.OrderBy)
            {
                case FilterTicketOrder.CreateDate_ASC:
                    query = query.OrderBy(x => x.CreateDate);
                    break;
                case FilterTicketOrder.CreateDate_DES:
                    query = query.OrderByDescending(x => x.CreateDate);
                    break;
            }

            #endregion

            #region filter

            if (filter.TicketSection != null)
                query = query.Where(x => x.TicketSection == filter.TicketSection.Value);
            if (filter.TicketPriority != null)
                query = query.Where(x => x.TicketPriority == filter.TicketPriority.Value);
            if (filter.UserId != null && filter.UserId != 0)
                query = query.Where(x => x.OwnerId == filter.UserId.Value);
            if (!string.IsNullOrEmpty(filter.Title))
                query = query.Where(x => EF.Functions.Like(x.Title, $"%{filter.Title}%"));

            #endregion

            #region Paging

            var pager = Pager.Build(filter.PageId, await query.CountAsync(), filter.TakeEntity,
                filter.ShowHowManyPageAfterAndBefore);
            var allEntities = await query.Paging(pager).ToListAsync();


            #endregion

            return filter.SetPaging(pager).SetTickets(allEntities);
        }

        public async Task<TicketDetailDto> GetTicketForShow(long ticketId, long userId)
        {
            var ticket = await _ticketRepository.GetQuery().Include(x => x.Owner)
                .FirstOrDefaultAsync(x => x.Id == ticketId);

            if (ticket == null || ticket.OwnerId != userId) return null;

            return new TicketDetailDto
            {
                Ticket = ticket,
                TicketMessages = await _ticketMessageRepository.GetQuery()
                    .OrderByDescending(x => x.CreateDate)
                    .Where(x => x.TicketId == ticketId && !x.IsDelete).ToListAsync()
            };
        }

        #endregion

        #region Contact Us

        public async Task CreateContactUs(CreateContactUsDto contact, string userIp, long? userId)
        {
            var newContact = new ContactUs
            {
                UserId = userId != null && userId.Value != 0 ? userId.Value : (long?)null,
                Subject = contact.Subject,
                FullName = contact.FullName,
                Email = contact.Email,
                Text = contact.Text,
                UserIp = userIp
            };

            await _contactUsRepository.AddEntity(newContact);
            await _contactUsRepository.SaveChanges();
        }

        #endregion

        #region Dispose

        public async ValueTask DisposeAsync()
        {
            await _contactUsRepository.DisposeAsync();
        }

        #endregion
    }
}
