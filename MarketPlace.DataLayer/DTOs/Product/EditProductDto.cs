namespace MarketPlace.DataLayer.DTOs.Product
{
    public class EditProductDto : CreateProductDto
    {
        public long Id { get; set; }
        public string ImageName { get; set; }
    }

    public enum EditProductResult
    {
        NotFound,
        NotForUser,
        Success
    }
}
