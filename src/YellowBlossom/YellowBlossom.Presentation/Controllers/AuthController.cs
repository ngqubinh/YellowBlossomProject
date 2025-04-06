using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YellowBlossom.Application.DTOs.Auth;
using YellowBlossom.Application.DTOs.General;
using YellowBlossom.Application.Interfaces.Auth;

namespace YellowBlossom.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService auth, ILogger<AuthController> logger)
        {
            this._auth = auth;
            this._logger = logger;
        }

        [HttpPost("sign-up")]
        public async Task<ActionResult<GeneralResponse>> SignUpAsync(SignUpRequest model)
        {
            var result = await this._auth.SignUpAsync(model);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("sign-in")]
        public async Task<ActionResult<UserResponse>> SignInAsync(SignInRequest model)
        {
            _logger.LogInformation("User sign-in attempt started. Request: {@SignInRequest}", model);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid sign-in request. Validation errors: {@ModelState}", ModelState);
                return BadRequest(ModelState);
            }

            try
            {
                UserResponse result = await _auth.SignInAsync(model);

                _logger.LogInformation("User signed in successfully. User: {@UserResponse}", result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Sign-in failed for request: {@SignInRequest}", model);
                return StatusCode(500, "An error occurred during sign-in.");
            }
        }

        [Authorize]
        [HttpGet("refresh-page")]
        public async Task<ActionResult<UserResponse>> RefreshPageAsync()
        {
            var user = await this._auth.RefreshPageAsync();
            return Ok(user);
        }

        [Authorize]
        [HttpPost("refresh-token")]
        public async Task<ActionResult<UserResponse>> RefreshTokenAsync()
        {
            return await this._auth.RefreshTokenAsync();
        }
    }
}
