using YellowBlossom.Domain.Models.Auth;

namespace YellowBlossom.Application.Interfaces.Auth
{
    public interface ITokenService
    {
        Task<string> GenerateAccessToken(User user);
        RefreshToken GenerateRefreshToken(User user);
    }
}
