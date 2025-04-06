using System.ComponentModel.DataAnnotations;

namespace YellowBlossom.Application.DTOs.Setting
{
    public class CreateInitAdminRequest
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
