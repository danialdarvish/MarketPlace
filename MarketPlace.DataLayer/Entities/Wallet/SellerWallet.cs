using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;
using MarketPlace.DataLayer.Entities.Common;
using MarketPlace.DataLayer.Entities.Store;

namespace MarketPlace.DataLayer.Entities.Wallet
{
    public class SellerWallet : BaseEntity
    {
        #region Properties

        public long SellerId { get; set; }
        public int Price { get; set; }
        public TransactionType TransactionType { get; set; }

        [Display(Name = "توضیحات")]
        [MaxLength(300, ErrorMessage = "نمی توان بیشتر از {1} کاراکتر باشد {0}")]
        public string Description { get; set; }

        #endregion

        #region Relations

        public Seller Seller { get; set; }


        #endregion
    }

    public enum TransactionType
    {
        Deposit,
        Withdrawal
    }
}
