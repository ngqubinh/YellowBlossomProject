using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Security.Claims;
using YellowBlossom.Application.DTOs.Auth;
using YellowBlossom.Application.Interfaces.Auth;
using YellowBlossom.Domain.Models.Auth;
using YellowBlossom.Infrastructure.Data;
using YellowBlossom.Infrastructure.Repositories.Auth;

namespace YellowBlossom.Testing.Auth
{
    
    public class AuthServiceTests
    {
        private Mock<UserManager<User>> _userManagerMock;
        private Mock<SignInManager<User>> _signInManagerMock;
        private Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private Mock<ITokenService> _tokenServiceMock;
        private ApplicationDbContext _context;
        private AuthService _authService;

        [SetUp]
        public void SetUp()
        {
            // Setup in-memory database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);

            // Mock UserManager
            _userManagerMock = MockUserManager<User>();
            // Mock SignInManager
            _signInManagerMock = MockSignInManager<User>();
            // Mock HttpContextAccessor
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            // Mock ITokenService
            _tokenServiceMock = new Mock<ITokenService>();

            _authService = new AuthService(
                _userManagerMock.Object,
                _context,
                _httpContextAccessorMock.Object,
                _signInManagerMock.Object,
                _tokenServiceMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        // Helper to mock UserManager
        private static Mock<UserManager<TUser>> MockUserManager<TUser>() where TUser : class
        {
            var store = new Mock<IUserStore<TUser>>();
            return new Mock<UserManager<TUser>>(
                store.Object, null, null, null, null, null, null, null, null);
        }

        // Helper to mock SignInManager
        private static Mock<SignInManager<TUser>> MockSignInManager<TUser>() where TUser : class
        {
            var contextAccessor = new Mock<IHttpContextAccessor>();
            var claimsFactory = new Mock<IUserClaimsPrincipalFactory<TUser>>();
            return new Mock<SignInManager<TUser>>(
                MockUserManager<TUser>().Object,
                contextAccessor.Object,
                claimsFactory.Object,
                null, null, null, null);
        }

        [Test]
        public async Task SignUpAsync_NewUser_SuccessfulRegistration()
        {
            // Arrange
            var request = new SignUpRequest
            {
                Email = "test@example.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!"
            };
            _userManagerMock.Setup(x => x.FindByEmailAsync(request.Email))
                .ReturnsAsync((User)null);
            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<User>(), request.Password))
                .ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _authService.SignUpAsync(request);

            // Assert
            result.Success.Should().BeTrue();
            result.Message.Should().Be("Register successfully.");
        }

        [Test]
        public async Task SignUpAsync_ExistingEmail_ReturnsError()
        {
            var request = new SignUpRequest
            {
                Email = "test@example.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!"
            };

            var existingUser = new User { Email = request.Email };
            this._userManagerMock.Setup(x => x.FindByEmailAsync(request.Email))
                .ReturnsAsync(existingUser);

            var result = await this._authService.SignUpAsync(request);

            result.Success.Should().BeFalse();
            result.Message.Should().Be($"{request.Email} is already existed");
        }

        [Test]
        public async Task SignUpAsync_WeakPassword_ReturnsError()
        {
            // Arrange
            var request = new SignUpRequest
            {
                Email = "test@example.com",
                Password = "weak",
                ConfirmPassword = "weak"
            };

            // Act
            var result = await _authService.SignUpAsync(request);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Password must include at least one letter, one number, and one special character.");
        }

        [Test]
        public async Task SignUpAsync_NullConfirmPassword_ReturnsError()
        {
            // Arrange
            var request = new SignUpRequest
            {
                Email = "test@example.com",
                Password = "Password123!",
                ConfirmPassword = string.Empty
            };

            // Act
            var result = await _authService.SignUpAsync(request);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("The ConfirmPassword field is required.");
        }

        [Test]
        public async Task SignUpAsync_RoleAssignmentFails_ReturnsError()
        {
            // Arrange
            var request = new SignUpRequest
            {
                Email = "test@example.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!"
            };
            _userManagerMock.Setup(x => x.FindByEmailAsync(request.Email))
                .ReturnsAsync((User)null);
            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<User>(), request.Password))
                .ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Role assignment failed" }));

            // Act
            var result = await _authService.SignUpAsync(request);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Assign role failed");
        }

        [Test]
        public async Task SignUpAsync_CreateUserFails_ReturnsError()
        {
            // Arrange
            var request = new SignUpRequest
            {
                Email = "test@example.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!"
            };
            _userManagerMock.Setup(x => x.FindByEmailAsync(request.Email))
                .ReturnsAsync((User)null);
            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<User>(), request.Password))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "User creation failed" }));

            // Act
            var result = await _authService.SignUpAsync(request);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("User creation failed");
        }

        [Test]
        public async Task SignInAsync_ValidCredentials_ReturnsUserResponseWithToken()
        {
            // Arrange
            var request = new SignInRequest
            {
                Email = "test@example.com",
                Password = "Password123!"
            };
            var user = new User
            {
                Email = request.Email,
                UserName = request.Email,
                RefreshTokens = new List<RefreshToken>() // Initialize RefreshTokens to avoid null
            };
            var accessToken = "mocked-jwt-token";
            _userManagerMock.Setup(x => x.FindByEmailAsync(request.Email))
                .ReturnsAsync(user);
            _signInManagerMock.Setup(x => x.CheckPasswordSignInAsync(user, request.Password, false))
                .ReturnsAsync(SignInResult.Success);
            _tokenServiceMock.Setup(x => x.GenerateAccessToken(user))
                .ReturnsAsync(accessToken);
            _tokenServiceMock.Setup(x => x.GenerateRefreshToken(user))
                .Returns(new RefreshToken
                {
                    Token = "mocked-refresh-token",
                    CreatedDateUTC = DateTime.UtcNow,
                    ExpiresDateUTC = DateTime.UtcNow.AddDays(7)
                });

            // Setup HttpContext (no longer needed for cookies, but kept for consistency)
            var signInHttpContext = new DefaultHttpContext();
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(signInHttpContext);

            // Act
            var result = await _authService.SignInAsync(request);

            // Assert
            result.Message.Should().BeNullOrEmpty(); // Indicates success
            result.Email.Should().Be(request.Email);
            result.AccessToken.Should().Be(accessToken);
        }

        [Test]
        public async Task SignInAsync_UserNotFound_ReturnsError()
        {
            // Arrange
            var request = new SignInRequest
            {
                Email = "test@example.com",
                Password = "Password123!"
            };
            _userManagerMock.Setup(x => x.FindByEmailAsync(request.Email))
                .ReturnsAsync((User)null);

            // Act
            var result = await _authService.SignInAsync(request);

            // Assert
            result.Message.Should().Be($"{request.Email} is not found");
            result.Email.Should().BeNull();
            result.AccessToken.Should().BeNull();
        }

        [Test]
        public async Task SignInAsync_InvalidPassword_ReturnsError()
        {
            // Arrange
            var request = new SignInRequest
            {
                Email = "test@example.com",
                Password = "WrongPassword"
            };
            var user = new User { Email = request.Email };
            _userManagerMock.Setup(x => x.FindByEmailAsync(request.Email))
                .ReturnsAsync(user);
            _signInManagerMock.Setup(x => x.CheckPasswordSignInAsync(user, request.Password, false))
                .ReturnsAsync(SignInResult.Failed);

            // Act
            var result = await _authService.SignInAsync(request);

            // Assert
            result.Message.Should().Be("The password is wrong");
            result.Email.Should().BeNull();
            result.AccessToken.Should().BeNull();
        }

        [Test]
        public async Task RefreshPageAsync_ValidUser_ReturnsUserResponseWithToken()
        {
            // Arrange
            var email = "test@example.com";
            var user = new User { Email = email, UserName = email };
            var accessToken = "mocked-jwt-token";
            var refreshPageHttpContext = new DefaultHttpContext();
            refreshPageHttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Email, email)
            }));
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(refreshPageHttpContext);
            _userManagerMock.Setup(x => x.FindByEmailAsync(email))
                .ReturnsAsync(user);
            _tokenServiceMock.Setup(x => x.GenerateAccessToken(user))
                .ReturnsAsync(accessToken);

            // Act
            var result = await _authService.RefreshPageAsync();

            // Assert
            result.Message.Should().BeNullOrEmpty(); // Indicates success
            result.Email.Should().Be(email);
            result.AccessToken.Should().Be(accessToken);
        }

        [Test]
        public async Task RefreshPageAsync_UserNotFound_ReturnsError()
        {
            // Arrange
            var email = "test@example.com";
            var userNotFoundHttpContext = new DefaultHttpContext();
            userNotFoundHttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Email, email)
            }));
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(userNotFoundHttpContext);
            _userManagerMock.Setup(x => x.FindByEmailAsync(email))
                .ReturnsAsync((User)null);

            // Act
            var result = await _authService.RefreshPageAsync();

            // Assert
            result.Message.Should().Be("User not found in the system.");
            result.Email.Should().BeNull();
            result.AccessToken.Should().BeNull();
        }
    }
}
