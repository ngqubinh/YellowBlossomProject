using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using YellowBlossom.Application.DTOs.Auth;
using YellowBlossom.Application.DTOs.General;
using YellowBlossom.Application.Interfaces.Auth;
using YellowBlossom.Presentation.Controllers;

namespace YellowBlossom.Testing.Auth
{
    [TestFixture]
    public class AuthControllerTests
    {
        private Mock<IAuthService> _authServiceMock;
        private Mock<ILogger<AuthController>> _loggerMock;
        private AuthController _controller;

        [SetUp]
        public void SetUp()
        {
            _authServiceMock = new Mock<IAuthService>();
            _loggerMock = new Mock<ILogger<AuthController>>();
            _controller = new AuthController(_authServiceMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task SignUpAsync_SuccessfulRegistration_ReturnsOk()
        {
            // Arrange
            var request = new SignUpRequest
            {
                Email = "test@example.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!"
            };
            var response = new GeneralResponse(true, "Register successfully.");
            _authServiceMock.Setup(x => x.SignUpAsync(request))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.SignUpAsync(request);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(response);
        }

        [Test]
        public async Task SignUpAsync_PasswordMismatch_ReturnsBadRequest()
        {
            // Arrange
            var request = new SignUpRequest
            {
                Email = "test@example.com",
                Password = "Password123!",
                ConfirmPassword = "DifferentPassword"
            };
            var response = new GeneralResponse(false, "'ConfirmPassword' and 'Password' do not match.");
            _authServiceMock.Setup(x => x.SignUpAsync(request))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.SignUpAsync(request);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult!.StatusCode.Should().Be(400);
            badRequestResult.Value.Should().BeEquivalentTo(response);
        }

        [Test]
        public async Task SignInAsync_ValidCredentials_ReturnsOk()
        {
            // Arrange
            var request = new SignInRequest
            {
                Email = "test@example.com",
                Password = "Password123!"
            };
            var response = new UserResponse
            {
                Email = request.Email,
                Message = "Sign-in successful"
            };
            _authServiceMock.Setup(x => x.SignInAsync(request))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.SignInAsync(request);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(response);
        }

        [Test]
        public async Task SignInAsync_InvalidCredentials_ReturnsOkWithErrorMessage()
        {
            // Arrange
            var request = new SignInRequest
            {
                Email = "test@example.com",
                Password = "WrongPassword"
            };
            var response = new UserResponse("The password is wrong");
            _authServiceMock.Setup(x => x.SignInAsync(request))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.SignInAsync(request);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(200);
            var userResponse = okResult.Value as UserResponse;
            userResponse.Should().NotBeNull();
            userResponse.Message.Should().Be("The password is wrong");
        }

        [Test]
        public async Task SignInAsync_InvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            var request = new SignInRequest
            {
                Email = "", // Invalid email
                Password = "Password123!"
            };
            _controller.ModelState.AddModelError("Email", "The Email field is required.");

            // Act
            var result = await _controller.SignInAsync(request);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult!.StatusCode.Should().Be(400);
        }

        [Test]
        public async Task SignInAsync_ValidRequest_LogsInformation()
        {
            // Arrange
            var request = new SignInRequest
            {
                Email = "test@example.com",
                Password = "Password123!"
            };
            var response = new UserResponse
            {
                Email = request.Email,
                Message = "Sign-in successful"
            };
            _authServiceMock.Setup(x => x.SignInAsync(request))
                .ReturnsAsync(response);

            // Act
            await _controller.SignInAsync(request);

            // Assert
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("User sign-in attempt started")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once());
        }
    }
}
