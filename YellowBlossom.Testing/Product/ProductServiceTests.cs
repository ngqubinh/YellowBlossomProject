//using FluentAssertions;
//using Microsoft.AspNetCore.Http;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Logging;
//using Moq;
//using System.Security.Claims;
//using YellowBlossom.Application.DTOs.Product;
//using YellowBlossom.Infrastructure.Data;
//using YellowBlossom.Infrastructure.Repositories.PMIS;

//namespace YellowBlossom.Testing.Product
//{
//    [TestFixture]
//    public class ProductServiceTests
//    {
//        private Mock<IHttpContextAccessor> _httpContextAccessorMock;
//        private Mock<ILogger<ProductService>> _loggerMock;
//        private ApplicationDbContext _dbContext;
//        private ProductService _productService;

//        [SetUp]
//        public void SetUp()
//        {
//            // Setup in-memory database
//            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
//                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
//                .Options;
//            _dbContext = new ApplicationDbContext(options);

//            // Mock HttpContextAccessor
//            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
//            // Mock ILogger
//            _loggerMock = new Mock<ILogger<ProductService>>();

//            _productService = new ProductService(_dbContext, _httpContextAccessorMock.Object, _loggerMock.Object);
//        }

//        [TearDown]
//        public void TearDown()
//        {
//            _dbContext.Database.EnsureDeleted();
//            _dbContext.Dispose();
//        }

//        [Test]
//        public async Task CreateProductAsync_ValidRequest_AdminUser_ReturnSuccess()
//        {
//            // Arrange
//            var request = new CreateProductRequest
//            {
//                ProductName = "Test Product",
//                Description = "Test Description"
//            };

//            string userId = "abc-xyz-12389";
//            var httpContext = new DefaultHttpContext();
//            httpContext.User = new System.Security.Claims.ClaimsPrincipal(new ClaimsIdentity(new[]
//            {
//                new Claim(ClaimTypes.NameIdentifier, userId),
//                new Claim(ClaimTypes.Role, "ADMIN")
//            }));

//            this._httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

//            // Act
//            var result = await this._productService.CreateProductAsync(request);

//            // Assert
//            result.Success.Should().BeTrue();
//            result.Message.Should().Be($"Product '{request.ProductName}' created successfully.");
//            var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.ProductName == request.ProductName);
//            product.Should().NotBeNull();
//            product!.ProductName.Should().Be(request.ProductName);
//            product.Description.Should().Be(request.Description);
//            product.CreatedBy.Should().Be(userId);
//        }

//        [Test]
//        public async Task CreateProductAsync_HttpContextUserNull_ReturnsError()
//        {
//            // Arrange
//            var request = new CreateProductRequest
//            {
//                ProductName = "Test Product",
//                Description = "Test Description"
//            };
//            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns((HttpContext)null!);

//            // Act
//            var result = await _productService.CreateProductAsync(request);

//            // Assert
//            result.Success.Should().BeFalse();
//            result.Message.Should().Be("User authentication failed.");

//            // Verify logging
//            _loggerMock.Verify(
//                x => x.Log(
//                    LogLevel.Warning,
//                    It.IsAny<EventId>(),
//                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("HttpContext or User is null.")),
//                    null,
//                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
//                Times.Once());
//        }
//        [Test]
//        public async Task CreateProductAsync_UserIdNotFound_ReturnsError()
//        {
//            // Arrange
//            var request = new CreateProductRequest
//            {
//                ProductName = "Test Product",
//                Description = "Test Description"
//            };
//            var httpContext = new DefaultHttpContext();
//            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity()); // no claim
//            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

//            // Act
//            var result = await _productService.CreateProductAsync(request);

//            // Assert
//            result.Success.Should().BeFalse();
//            result.Message.Should().Be("User not authenticated.");

//            // Verify logging
//            _loggerMock.Verify(
//                x => x.Log(
//                    LogLevel.Warning,
//                    It.IsAny<EventId>(),
//                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("User ID not found in HttpContext.")),
//                    null,
//                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
//                Times.Once());
//        }

//        [Test]
//        public async Task CreateProductAsync_UserNotAdmin_ReturnsError()
//        {
//            // Arrange
//            var request = new CreateProductRequest
//            {
//                ProductName = "Test Product",
//                Description = "Test Description"
//            };
//            var userId = "user123";
//            var httpContext = new DefaultHttpContext();
//            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
//            {
//                new Claim(ClaimTypes.NameIdentifier, userId),
//                new Claim(ClaimTypes.Role, "USER") // Not ADMIN
//            }));
//            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

//            // Act
//            var result = await _productService.CreateProductAsync(request);

//            // Assert
//            result.Success.Should().BeFalse();
//            result.Message.Should().Be("You do not have permission to create a product.");

//            var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.ProductName == request.ProductName);
//            product.Should().BeNull(); // Product should not be created
//        }
//    }
//}
