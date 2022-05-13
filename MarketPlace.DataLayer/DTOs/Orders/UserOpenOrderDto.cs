using System.Collections.Generic;
using System.Linq;

namespace MarketPlace.DataLayer.DTOs.Orders
{
    public class UserOpenOrderDto
    {
        public long UserId { get; set; }
        public string Description { get; set; }
        public List<UserOpenOrderDetailItemDto> Details { get; set; }

        public int GetTotalPrice()
        {
            return Details.Sum(x => (x.ProductPrice + x.ProductColorPrice) * x.Count);
        }


        public int GetTotalDiscount()
        {
            return Details.Sum(x => x.GetOrderDetailWithDiscountPriceAmount());
        }
    }
}
