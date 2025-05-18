using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog.Context;
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
            LogContext.PushProperty("ServiceName", "ProductService");
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

                string productName = model.ProductName.Trim().ToLower();
                bool productExists = await this._dbContext.Products
                    .AnyAsync(p => p.ProductName.ToLower() == productName);

                if (productExists)
                {
                    this._logger.LogError("Product with name {ProductName} already exists.", model.ProductName);
                    return new GeneralResponse(false, $"Product with name '{model.ProductName}' already exists.");
                }

                PMIS_Product newProduct = new PMIS_Product(productName, model.Description, userId);
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

                string? userId = this._http.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrWhiteSpace(userId))
                {
                    this._logger.LogError("User ID not found in HttpContext.");
                    return new ProductDTO { Message = "User ID not found in HttpContext." };
                }

                if (!this._http.HttpContext.User.IsInRole(StaticUserRole.ADMIN))
                {
                    this._logger.LogError("User does not have permission to edit product.");
                    return new ProductDTO { Message = "User does not have permission to edit product." };
                }

                if (model == null)
                {
                    this._logger.LogError("EditProductRequest model is null");
                    return new ProductDTO { Message = "EditProductRequest model is null" };
                }

                PMIS_Product? product = await this._dbContext.Products.FirstOrDefaultAsync(p => p.ProductId == productId);
                if (product == null)
                {
                    this._logger.LogError($"Product with ID {productId} not found.");
                    return new ProductDTO { Message = $"Product with ID {productId} not found." };
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
                    return new ProductDTO { Message = $"Invalid HTTP context or user for product {productId}" };
                }

                bool isAdmin = this._http.HttpContext!.User.IsInRole(StaticUserRole.ADMIN);
                if (isAdmin == true)
                {
                    PMIS_Product? product = await this._dbContext.Products.FirstOrDefaultAsync(p => p.ProductId == productId);
                    if (product == null)
                    {
                        this._logger.LogInformation("There is no product data.");
                        return new ProductDTO { Message = "There is not product data." };
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
                this._logger.LogWarning("User lacks admin role to track product {ProductId}", productId);
                return new ProductDTO { Message = $"User lacks admin role to track product {productId}" };
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error tracking product {ProductId}", productId);
                return new ProductDTO { Message =  ex.Message };
            }
        }

        public async Task<List<ProductDTO>> GetProductsAsync()
        {
            try
            {
                if (GeneralService.IsHttpContextOrUserNull(this._http.HttpContext!))
                {
                    this._logger.LogError("HttpContext or User is null.");
                    return new List<ProductDTO>
                    {
                        new ProductDTO { Message = "HttpContext or User is null." }
                    };
                }

                string? userId = this._http.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrWhiteSpace(userId))
                {
                    this._logger.LogError("User ID not found in HttpContext.");
                    return new List<ProductDTO>{
                        new ProductDTO { Message = "User ID not found in HttpContext."}
                    };
                }

                //string query = "SELECT \"ProductId\", \"ProductName\", \"Version\", \"CreatedAt\" FROM public.\"Products\" WHERE \"CreatedBy\" = {0}";
                //List<PMIS_Product> products = await this._dbContext.Products
                //    .FromSqlRaw(query, userId)
                //    .ToListAsync();
                List<PMIS_Product> products = await this._dbContext.Products
                    .Where(p => p.CreatedBy == userId)
                    .Include(p => p.User)
                    .ToListAsync();
                if (products == null)
                {
                    this._logger.LogInformation("There is not products information. Please check again!");
                    return new List<ProductDTO>
                    {
                        new ProductDTO { Message = "There is not products information. Please check again!" }
                    };
                }

                List<ProductDTO> productDTOs = Mapper.MapProductToProductDTOByList(products);
                return productDTOs;
            }
            catch (Exception ex)
            {
                this._logger.LogError($"Error: {ex.Message}");
                return new List<ProductDTO> { new ProductDTO { Message = ex.Message } };
            }
        }

        public async Task<GeneralResponse> DeleteProductAsync(Guid productId)
        {
            try
            {
                // Kiểm tra HttpContext
                if (GeneralService.IsHttpContextOrUserNull(this._http.HttpContext!))
                {
                    Console.WriteLine("HttpContext or User is null.");
                    return new GeneralResponse(false, "HttpContext or Uesr is null.");
                }

                // Lấy UserId từ HttpContext
                string? userId = GeneralService.GetUserIdFromContext(this._http.HttpContext!);
                if (string.IsNullOrEmpty(userId))
                {
                    Console.WriteLine("User ID not found in HttpContext.");
                    return new GeneralResponse(false, "User ID not found in HttpContext.");
                }

                if (!this._http.HttpContext!.User.IsInRole(StaticUserRole.ADMIN))
                {
                    Console.WriteLine("User does not have permission to delete product.");
                    return new GeneralResponse(false, "You do not have permission to delete a product.");
                }

                PMIS_Product? product = await this._dbContext.Products
                    .Where(p => p.ProductId == productId)
                    .FirstOrDefaultAsync();
                if (product == null)
                {
                    Console.WriteLine("Product not found.");
                    return new GeneralResponse(false, "Product not found.");
                }

                this._dbContext.Products.Remove(product);
                await this._dbContext.SaveChangesAsync();

                return new GeneralResponse(true, "Deleted product successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new GeneralResponse(false, ex.Message);
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

        #endregion
    }
}
