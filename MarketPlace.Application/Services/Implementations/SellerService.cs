using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarketPlace.Application.Services.Interfaces;
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

        #endregion

        #region Dispose

        public async ValueTask DisposeAsync()
        {
            await _sellerRepository.DisposeAsync();
        }

        #endregion
    }
}
