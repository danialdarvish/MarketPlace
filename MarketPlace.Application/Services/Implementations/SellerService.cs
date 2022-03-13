using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarketPlace.Application.Services.Interfaces;
using MarketPlace.DataLayer.DTOs.Paging;
using MarketPlace.DataLayer.DTOs.Seller;
using MarketPlace.DataLayer.Entities.Account;
using MarketPlace.DataLayer.Entities.Store;
using MarketPlace.DataLayer.Repository;
using Microsoft.EntityFrameworkCore;

namespace MarketPlace.Application.Services.Implementations
{
    public class SellerService : ISellerService
    {
        #region Constructor

        private readonly IGenericRepository<User> _userRepository;
        private readonly IGenericRepository<Seller> _sellerRepository;

        public SellerService(IGenericRepository<Seller> sellerRepository, IGenericRepository<User> userRepository)
        {
            _userRepository = userRepository;
            _sellerRepository = sellerRepository;
        }

        #endregion

        #region Seller

        public async Task<RequestSellerResult> AddNewSellerRequest(RequestSellerDto seller, long userId)
        {
            var user = await _userRepository.GetEntityById(userId);
            if (user.IsBlocked) return RequestSellerResult.HasNotPermission;

            var hasUnderProgressRequest = await _sellerRepository.GetQuery().AnyAsync(x =>
                x.UserId == userId && x.StoreAcceptanceState == StoreAcceptanceState.UnderProgress);

            if (hasUnderProgressRequest) return RequestSellerResult.HasUnderProgressRequest;

            var newSeller = new Seller
            {
                UserId = userId,
                StoreName = seller.StoreName,
                Address = seller.Address,
                Phone = seller.Phone,
                StoreAcceptanceState = StoreAcceptanceState.UnderProgress
            };

            await _sellerRepository.AddEntity(newSeller);
            await _sellerRepository.SaveChanges();

            return RequestSellerResult.Success;
        }

        public async Task<FilterSellerDto> FilterSellers(FilterSellerDto filter)
        {
            var query = _sellerRepository.GetQuery()
                .Include(x => x.User).AsQueryable();

            #region State

            switch (filter.State)
            {
                case FilterSellerState.All:
                    query = query.Where(x => !x.IsDelete);
                    break;
                case FilterSellerState.UnderProgress:
                    query = query.Where(x => x.StoreAcceptanceState == StoreAcceptanceState.UnderProgress && !x.IsDelete);
                    break;
                case FilterSellerState.Accepted:
                    query = query.Where(x => x.StoreAcceptanceState == StoreAcceptanceState.Accepted && !x.IsDelete);
                    break;
                case FilterSellerState.Rejected:
                    query = query.Where(x => x.StoreAcceptanceState == StoreAcceptanceState.Rejected && !x.IsDelete);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            #endregion

            #region Filter

            if (filter.UserId != null && filter.UserId != 0)
                query = query.Where(x => x.UserId == filter.UserId);
            if (!string.IsNullOrEmpty(filter.StoreName))
                query = query.Where(x => EF.Functions.Like(x.StoreName, $"%{filter.StoreName}%"));
            if (!string.IsNullOrEmpty(filter.Phone))
                query = query.Where(x => EF.Functions.Like(x.Phone, $"%{filter.Phone}%"));
            if (!string.IsNullOrEmpty(filter.Mobile))
                query = query.Where(x => EF.Functions.Like(x.Mobile, $"%{filter.Mobile}%"));
            if (!string.IsNullOrEmpty(filter.Address))
                query = query.Where(x => EF.Functions.Like(x.Address, $"%{filter.Address}%"));

            #endregion

            #region Paging

            var pager = Pager.Build(filter.PageId, await query.CountAsync(), filter.TakeEntity,
                filter.ShowHowManyPageAfterAndBefore);
            var allEntities = await query.Paging(pager).ToListAsync();

            #endregion

            return filter.SetPaging(pager).SetSellers(allEntities);
        }

        public async Task<EditRequestSellerDto> GetRequestSellerForEdit(long id, long currentUserId)
        {
            var seller = await _sellerRepository.GetEntityById(id);
            if (seller == null || seller.UserId != currentUserId) return null;

            return new EditRequestSellerDto
            {
                Id = seller.Id,
                Phone = seller.Phone,
                Address = seller.Address,
                StoreName = seller.StoreName
            };
        }

        public async Task<EditRequestSellerResult> EditRequestSeller(EditRequestSellerDto request, long currentUserId)
        {
            var seller = await _sellerRepository.GetEntityById(request.Id);
            if (seller == null || seller.UserId != currentUserId) return EditRequestSellerResult.NotFound;

            seller.Phone = request.Phone;
            seller.Address = request.Address;
            seller.StoreName = request.StoreName;
            seller.StoreAcceptanceState = StoreAcceptanceState.UnderProgress;

            _sellerRepository.EditEntity(seller);
            await _sellerRepository.SaveChanges();

            return EditRequestSellerResult.Success;
        }

        public async Task<bool> AcceptSellerRequest(long requestId)
        {
            var sellerRequest = await _sellerRepository.GetEntityById(requestId);
            if (sellerRequest != null)
            {
                sellerRequest.StoreAcceptanceState = StoreAcceptanceState.Accepted;
                _sellerRepository.EditEntity(sellerRequest);
                await _sellerRepository.SaveChanges();

                return true;
            }

            return false;
        }

        #endregion

        #region Dispose

        public async ValueTask DisposeAsync()
        {
            await _sellerRepository.DisposeAsync();
        }

        #endregion
    }
}
