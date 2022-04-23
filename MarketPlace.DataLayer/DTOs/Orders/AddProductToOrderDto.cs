namespace MarketPlace.DataLayer.DTOs.Orders
{
    public class AddProductToOrderDto
    {
        public long ProductId { get; set; }
        public long? ProductColorId { get; set; }
        public int Count { get; set; }
    }
}
