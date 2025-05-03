using YellowBlossom.Application.DTOs.General;
using YellowBlossom.Domain.Models.PMIS;

namespace YellowBlossom.Application.DTOs.Auth
{
    public class UserResponse
    {
        public string? AccessToken { get; set; }
        public string? Message { get; set; }
        public string? Email { get; set; }
        public List<ProductDTO> Products { get; set; } = new List<ProductDTO>();

        // Constructors
        public UserResponse() { }
        public UserResponse(string message)
        {
            this.Message = message;
        }
    }
}
