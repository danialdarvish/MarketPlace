namespace MarketPlace.DataLayer.DTOs.Orders
{
    public class UserOpenOrderDetailItemDto
    {
        public long ProductId { get; set; }
        public string ProductTitle { get; set; }
        public string ProductImageName { get; set; }
        public long? ProductColorId { get; set; }
        public int Count { get; set; }
        public int ProductPrice { get; set; }
        public int ProductColorPrice { get; set; }
        public string ColorName { get; set; }
    }
}