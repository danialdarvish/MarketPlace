using System.ComponentModel.DataAnnotations;

namespace MarketPlace.DataLayer.DTOs.Product_Discount
{
    public class CreateProductDiscountDto
    {
        public long ProductId { get; set; }

        [Range(0, 100)]
        public int Percentage { get; set; }

        public string ExpireDate { get; set; }
        public int DiscountNumber { get; set; }
    }

    public enum CreateDiscountResult
    {
        Success,
        ProductIsNotForSeller,
        ProductNotFound,
        Error
    }
}
