namespace YellowBlossom.Application.DTOs.General
{
    public class ProductDTO
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Version { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdated { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public UserDTO? User { get; set; }
        public string? Message { get; set; }
    }
}
