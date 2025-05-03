using YellowBlossom.Application.DTOs.Auth;
using YellowBlossom.Application.DTOs.General;

namespace YellowBlossom.Application.Interfaces.Auth
{
    public interface IAuthService
    {
        Task<GeneralResponse> SignUpAsync(SignUpRequest model);
        Task<UserResponse> SignInAsync(SignInRequest model);
        Task<UserResponse> RefreshPageAsync();
        Task<UserResponse> RefreshTokenAsync();
    }
}
