using System;
using System.Threading.Tasks;
using MarketPlace.DataLayer.DTOs.Common;
using MarketPlace.DataLayer.DTOs.Seller;

namespace MarketPlace.Application.Services.Interfaces
{
    public interface ISellerService : IAsyncDisposable
    {
        #region Seller

        Task<RequestSellerResult> AddNewSellerRequest(RequestSellerDto seller, long userId);
        Task<FilterSellerDto> FilterSellers(FilterSellerDto filter);
        Task<EditRequestSellerDto> GetRequestSellerForEdit(long id, long currentUserId);
        Task<EditRequestSellerResult> EditRequestSeller(EditRequestSellerDto request, long currentUserId);
        Task<bool> AcceptSellerRequest(long requestId);
        Task<bool> RejectSellerRequest(RejectItemDto reject);

        #endregion
    }
}
