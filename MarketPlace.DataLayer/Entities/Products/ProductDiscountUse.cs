using MarketPlace.DataLayer.Entities.Account;
using MarketPlace.DataLayer.Entities.Common;

namespace MarketPlace.DataLayer.Entities.Products
{
    public class ProductDiscountUse : BaseEntity
    {
        #region Properties

        public long UserId { get; set; }
        public long ProductDiscountId { get; set; }

        #endregion

        #region Relations

        public User User { get; set; }
        public ProductDiscount ProductDiscount { get; set; }

        #endregion
    }
}
