using System;
using System.Threading.Tasks;
using MarketPlace.DataLayer.DTOs.SellerWallet;
using MarketPlace.DataLayer.Entities.Wallet;

namespace MarketPlace.Application.Services.Interfaces
{
    public interface ISellerWalletService : IAsyncDisposable
    {
        #region Wallet

        Task<FilterSellerWalletDto> FilterSellerWalletDto(FilterSellerWalletDto filter);
        Task AddWallet(SellerWallet wallet);

        #endregion
    }
}
