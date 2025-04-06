using System.ComponentModel.DataAnnotations;

namespace YellowBlossom.Application.DTOs.Product
{
    public class CreateProductRequest
    {
        [Required]
        public string? ProductName { get; set; } = string.Empty;

        public string? Description { get; set; }
    }
}
