using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using YellowBlossom.Application.DTOs.Auth;
using YellowBlossom.Application.DTOs.General;
using YellowBlossom.Application.Interfaces.Auth;
using YellowBlossom.Domain.Constants;
using YellowBlossom.Domain.Models.Auth;
using YellowBlossom.Infrastructure.Data;
using YellowBlossom.Infrastructure.Services;

namespace YellowBlossom.Infrastructure.Repositories.Auth
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _http;
        private readonly SignInManager<User> _signInManager;
        private readonly ITokenService _token;

        public AuthService(UserManager<User> userManager, ApplicationDbContext context, IHttpContextAccessor http, SignInManager<User> signInManager, ITokenService token)
        {
            _userManager = userManager;
            _context = context;
            _http = http;
            _signInManager = signInManager;
            _token = token;
        }

        public async Task<UserResponse> RefreshPageAsync()
        {
            var getUserFromCookie = _http.HttpContext!.User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(getUserFromCookie))
                return new UserResponse("User email not found in cookie or session.");
            var user = await _userManager.FindByEmailAsync(getUserFromCookie);
            if (user == null) return new UserResponse("User not found in the system.");
            return await DisplayUserResponse(user);
        }

        public async Task<UserResponse> RefreshTokenAsync()
        {
            try
            {
                string? token = _http.HttpContext!.Request.Cookies["token"];
                string? userId = _http.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var check = IsValidRefreshTokenAsync(userId!, token!).GetAwaiter().GetResult();
                var user = await _userManager.FindByIdAsync(userId!);
                if (user == null) return new UserResponse("Token Refreshing failed.");
                return await DisplayUserResponse(user);
            }
            catch (Exception ex)
            {
                return new UserResponse(ex.Message.ToString());
            }
        }

        public async Task<UserResponse> SignInAsync(SignInRequest model)
        {
            User? user = await FindUserByEmailAsync(model.Email);
            if (user == null) return new UserResponse($"{model.Email} is not found");

            SignInResult result = await _signInManager.CheckPasswordSignInAsync(user, model.Password!, false);
            Console.WriteLine("SignIn result: Succeeded={0}", result.Succeeded);
            if (!result.Succeeded) return new UserResponse("The password is wrong");

            return await DisplayUserResponse(user);
        }

        public async Task<GeneralResponse> SignUpAsync(SignUpRequest model)
        {
            try
            {
                User? user = await FindUserByEmailAsync(model.Email);
                if (user != null)
                    return new GeneralResponse(false, $"{model.Email} is already existed");

                if (string.IsNullOrEmpty(model.Password) || !model.Password.Any(char.IsDigit) ||
                  !model.Password.Any(char.IsLetter) ||
                  !model.Password.Any(ch => !char.IsLetterOrDigit(ch)))
                    return new GeneralResponse(false, "Password must include at least one letter, one number, and one special character.");

                if (string.IsNullOrEmpty(model.ConfirmPassword))
                    return new GeneralResponse(false, "The ConfirmPassword field is required.");

                if (model.Password != model.ConfirmPassword)
                    return new GeneralResponse(false, "'ConfirmPassword' and 'Password' do not match.");

                User newUser = new User()
                {
                    UserName = model.Email,
                    Email = model.Email,
                    CreatedAt = DateTime.UtcNow,
                };

                var result = await _userManager.CreateAsync(newUser, model.Password);
                string err = CheckResponse(result);
                if (!string.IsNullOrEmpty(err))
                    return new GeneralResponse(false, err);

                var assignRole = await _userManager.AddToRoleAsync(newUser, StaticUserRole.Unknown);
                if (!assignRole.Succeeded)
                    return new GeneralResponse(false, "Assign role failed");

                return new GeneralResponse(true, "Register successfully.");
            }
            catch (Exception ex)
            {
                return new GeneralResponse(false, ex.Message.ToString());
            }
        }

        #region extra functions
        private async Task<User?> FindUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        private static string CheckResponse(IdentityResult result)
        {
            if (!result.Succeeded)
            {
                var err = result.Errors.Select(i => i.Description);
                return string.Join(Environment.NewLine, err);
            }

            return null!;
        }

        private async Task<UserResponse> DisplayUserResponse(User user)
        {
            await SaveRefreshToken(user);
            var accessToken = await _token.GenerateAccessToken(user);
            var products = await this._context.Products
                .Where(p => p.CreatedBy == user.Id).ToListAsync();
            var productDTOs = Mapper.MapProductToProductDTOByList(products);
            return new UserResponse()
            {
                AccessToken = accessToken,
                Message = "",
                Email = user.Email,
                //Products = productDTOs
            };
        }

        private async Task SaveRefreshToken(User user)
        {
            var refreshToken = _token.GenerateRefreshToken(user);
            var existsRefreshToken = user.RefreshTokens.FirstOrDefault();
            if (existsRefreshToken != null)
            {
                existsRefreshToken.Token = refreshToken.Token;
                existsRefreshToken.CreatedDateUTC = refreshToken.CreatedDateUTC;
                existsRefreshToken.ExpiresDateUTC = refreshToken.ExpiresDateUTC;
            }
            else
            {
                user.RefreshTokens.Add(refreshToken);
            }
            await _context.SaveChangesAsync();

            var cookieOptions = new CookieOptions()
            {
                Expires = refreshToken.ExpiresDateUTC,
                HttpOnly = true, // Blocks JavaScript access (XSS protection)
                IsEssential = true, // Bypasses consent checks
                Path = "/", // Available on all routes

                SameSite = SameSiteMode.Lax,
                Secure = false,
            };

            _http.HttpContext!.Response.Cookies.Append("token", refreshToken.Token!, cookieOptions);
        }

        private async Task<bool> IsValidRefreshTokenAsync(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token)) return false;

            var fetchedRefreshToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(x => x.UserId == userId && x.Token == token);
            if (fetchedRefreshToken == null) return false;
            //if (fetchedRefreshToken.IsExpired) return false;

            return true;
        }
        #endregion
    }
}
