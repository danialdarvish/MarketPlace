using MarketPlace.Application.Services.Interfaces;
using MarketPlace.DataLayer.DTOs.Contacts;
using MarketPlace.DataLayer.Entities.Contacts;
using MarketPlace.DataLayer.Repository;
using System.Threading.Tasks;

namespace MarketPlace.Application.Services.Implementations
{
    public class ContactService : IContactService
    {
        #region Constructor
        private readonly IGenericRepository<ContactUs> _contactUsRepository;

        public ContactService(IGenericRepository<ContactUs> contactUsRepository)
        {
            _contactUsRepository = contactUsRepository;
        }
        #endregion

        #region Contact Us
        public async Task CreateContactUs(CreateContactUsDto contact, string userIp, long? userId)
        {
            var newContact = new ContactUs
            {
                UserId = userId != null && userId.Value != 0 ? userId.Value : (long?) null,
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
