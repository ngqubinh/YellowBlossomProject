namespace YellowBlossom.Application.DTOs.Product
{
    public class EditProductRequest : CreateProductRequest
    {
        public string Version { get; set; } = string.Empty;
        public DateTime LastUpdated { get; set; }
    }
}
