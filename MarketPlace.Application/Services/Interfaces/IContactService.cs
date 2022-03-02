using MarketPlace.DataLayer.DTOs.Contacts;
using System;
using System.Threading.Tasks;

namespace MarketPlace.Application.Services.Interfaces
{
    public interface IContactService : IAsyncDisposable
    {
        Task CreateContactUs(CreateContactUsDto contact, string userIp, long? userId);
    }
}
