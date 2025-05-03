using System.ComponentModel.DataAnnotations;

namespace YellowBlossom.Application.DTOs.Auth
{
    public class SignUpRequest : SignInRequest
    {
        [Required, Compare(nameof(Password))]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
