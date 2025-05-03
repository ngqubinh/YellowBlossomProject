using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using YellowBlossom.Application.Interfaces.Auth;
using YellowBlossom.Domain.Models.Auth;

namespace YellowBlossom.Infrastructure.Repositories.Auth
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly SymmetricSecurityKey _jwtKey;
        private readonly UserManager<User> _userManager;

        public TokenService(IConfiguration config, UserManager<User> userManager)
        {
            _config = config;
            _userManager = userManager;
            _jwtKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Key"]!));
        }

        public async Task<string> GenerateAccessToken(User user)
        {
            string role = (await _userManager.GetRolesAsync(user)).FirstOrDefault()!.ToString();
            var userClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Role, role)
            };

            SigningCredentials credentials = new SigningCredentials(_jwtKey, SecurityAlgorithms.HmacSha256);
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(userClaims),
                Expires = DateTime.UtcNow.AddDays(int.Parse(_config["JWT:ExpiresInMinutes"]!)),
                SigningCredentials = credentials,
                Issuer = _config["JWT:Issuer"],
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken jwt = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(jwt);
        }

        public RefreshToken GenerateRefreshToken(User user)
        {
            var tokenByte = new byte[32];
            using var randomNumber = RandomNumberGenerator.Create();
            randomNumber.GetBytes(tokenByte);

            string token = Convert.ToBase64String(tokenByte);
            DateTime dateExpiresUTC = DateTime.UtcNow.AddDays(int.Parse(_config["JWT:RefreshTokenExpiresInDays"]!));

            RefreshToken refreshToken = new RefreshToken(token, user.Id, dateExpiresUTC);
            return refreshToken;
        }
    }
}
