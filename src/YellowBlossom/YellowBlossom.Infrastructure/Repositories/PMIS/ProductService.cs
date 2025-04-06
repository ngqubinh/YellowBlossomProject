using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using YellowBlossom.Application.DTOs.General;
using YellowBlossom.Application.DTOs.Product;
using YellowBlossom.Application.Interfaces.PMIS;
using YellowBlossom.Domain.Constants;
using YellowBlossom.Domain.Models.PMIS;
using YellowBlossom.Infrastructure.Data;
using YellowBlossom.Infrastructure.Services;

namespace YellowBlossom.Infrastructure.Repositories.PMIS
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _http;
        private readonly ILogger<ProductService> _logger;

        public ProductService(ApplicationDbContext dbContext, IHttpContextAccessor http, ILogger<ProductService> logger)
        {
            this._dbContext = dbContext;
            this._http = http;
            this._logger = logger;
        }

        public async Task<GeneralResponse> CreateProductAsync(CreateProductRequest model)
        {
            try
            {
                if (this._http.HttpContext?.User == null)
                {
                    _logger.LogWarning("HttpContext or User is null.");
                    return new GeneralResponse(false, "User authentication failed.");
                }

                string? userId = this._http.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrWhiteSpace(userId))
                {
                    _logger.LogWarning("User ID not found in HttpContext.");
                    return new GeneralResponse(false, "User not authenticated.");
                }

                if(!this._http.HttpContext.User.IsInRole(StaticUserRole.ADMIN))
                {
                    Console.WriteLine("User does not have permission to create product.");
                    return new GeneralResponse(false, "You do not have permission to create a product.");
                }

                Product newProduct = new Product(model.ProductName, model.Description, userId);
                this._dbContext.Products.Add(newProduct);
                await this._dbContext.SaveChangesAsync();

                ProductDTO productDTO = Mapper.MapProductToProductDTO(newProduct);
                return new GeneralResponse(true, $"Product '{productDTO.ProductName}' created successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create product for request {@Request}", model);
                return new GeneralResponse(false, "An error occurred while creating the product.");
            }
        }

        public async Task<ProductDTO> EditProductAsync(Guid productId, EditProductRequest model)
        {
            try
            {
                bool isUserAndContextNull = CheckHttpContextAndUser();
                if (!isUserAndContextNull)
                    return null!;

                string? userId = this._http.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrWhiteSpace(userId))
                {
                    Console.WriteLine("User ID not found in HttpContext.");
                    return null!;
                }

                if (!this._http.HttpContext.User.IsInRole(StaticUserRole.ADMIN))
                {
                    Console.WriteLine("User does not have permission to create product.");
                    return null!;
                }

                if (model == null)
                {
                    Console.WriteLine("EditProductRequest model is null");
                    return null!;
                }

                Product? product = await this._dbContext.Products.FirstOrDefaultAsync(p => p.ProductId == productId);
                if (product == null)
                {
                    Console.WriteLine($"Product with ID {productId} not found.");
                    return null!;
                }

                product.ProductName = string.IsNullOrEmpty(model.ProductName) ? product.ProductName : model.ProductName;
                product.Description = string.IsNullOrEmpty(model.Description) ? product.Description : model.Description;
                product.Version = string.IsNullOrEmpty(model.Version) ? product.Version : model.Version;
                product.LastUpdated = model.LastUpdated = DateTime.UtcNow;
                await this._dbContext.SaveChangesAsync();

                ProductDTO productDTO = Mapper.MapProductToProductDTO(product);
                return productDTO;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null!;
            }
        }

        public async Task<ProductDTO> TrackProductAsync(Guid productId)
        {
            try
            {
                bool isUserAndContextNull = CheckHttpContextAndUser();
                if (!isUserAndContextNull)
                {
                    this._logger.LogWarning("Invalid HTTP context or user for product {ProductId}", productId);
                    return null!;
                }

                bool isAdmin = CheckAdminRole();
                if (isAdmin)
                {
                    Product? product = await this._dbContext.Products.FirstOrDefaultAsync(p => p.ProductId == productId);
                    if (product == null)
                    {
                        this._logger.LogWarning("User lacks admin role to track product {ProductId}", productId);
                        return null!;
                    }

                    return new ProductDTO
                    {
                        ProductId = productId,
                        ProductName = product.ProductName,
                        Description = product.Description,
                        CreatedAt = product.CreatedAt,
                        CreatedBy = product.CreatedBy,
                        LastUpdated = product.LastUpdated,
                        Version = product.Version
                    };
                }
                return null!;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error tracking product {ProductId}", productId);
        throw;
            }
        }

        #region extra functions
        protected bool CheckHttpContextAndUser()
        {
            if(this._http.HttpContext?.User == null)
            {
                Console.WriteLine("HttpContext or User is null.");
                return false;
            }
            return true;
        }

        protected bool CheckAdminRole()
        {
            if (_http.HttpContext?.User?.IsInRole(StaticUserRole.ADMIN) != true)
            {
                Console.WriteLine("User does not have permission to create product.");
                return false;
            }
            return true;
        }
        #endregion
    }
}
