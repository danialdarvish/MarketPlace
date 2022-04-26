using System.Collections.Generic;
using MarketPlace.DataLayer.DTOs.Paging;

namespace MarketPlace.DataLayer.DTOs.SellerWallet
{
    public class FilterSellerWalletDto : BasePaging
    {
        public long? SellerId { get; set; }
        public int? PriceFrom { get; set; }
        public int? PriceTo { get; set; }
        public List<Entities.Wallet.SellerWallet> SellerWallets { get; set; }

        public FilterSellerWalletDto SetSellerWallets(List<Entities.Wallet.SellerWallet> wallets)
        {
            SellerWallets = wallets;
            return this;
        }

        public FilterSellerWalletDto SetPaging(BasePaging paging)
        {
            PageId = paging.PageId;
            AllEntitiesCount = paging.AllEntitiesCount;
            StartPage = paging.StartPage;
            EndPage = paging.EndPage;
            ShowHowManyPageAfterAndBefore = paging.ShowHowManyPageAfterAndBefore;
            TakeEntity = paging.TakeEntity;
            SkipEntity = paging.SkipEntity;
            PageCount = paging.PageCount;
            return this;
        }
    }
}
