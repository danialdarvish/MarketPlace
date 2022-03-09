using System;
using System.Threading.Tasks;
using MarketPlace.DataLayer.DTOs.Seller;

namespace MarketPlace.Application.Services.Interfaces
{
    public interface ISellerService : IAsyncDisposable
    {
        #region Seller

        Task<RequestSellerResult> AddNewSellerRequest(RequestSellerDto seller, long userId);

        #endregion
    }
}
