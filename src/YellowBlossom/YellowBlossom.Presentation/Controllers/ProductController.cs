using Microsoft.AspNetCore.Mvc;
using YellowBlossom.Application.DTOs.General;
using YellowBlossom.Application.DTOs.Product;
using YellowBlossom.Application.Interfaces.PMIS;

namespace YellowBlossom.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _product;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IProductService product, ILogger<ProductController> logger)
        {
            this._product = product;
            this._logger = logger;
        }

        [HttpPost("create-product")]
        public async Task<ActionResult<GeneralResponse>> CreateProductAsync(CreateProductRequest model)
        {
            GeneralResponse ressult = await this._product.CreateProductAsync(model);
            if(!ModelState.IsValid || ressult.Success==false)
            {
                return BadRequest(ressult);
            }
            return Ok(ressult);
        }

        [HttpPost("edit-product")]
        public async Task<ActionResult<ProductDTO>> EditProductAsync(Guid productId, EditProductRequest model)
        {
            ProductDTO result = await this._product.EditProductAsync(productId, model);
            if(!ModelState.IsValid || result == null)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpGet("track-product")]
        public async Task<ActionResult<ProductDTO>> TrackProductAsync(Guid productId)
        {
            
            if (productId == Guid.Empty)
            {
                this._logger.LogWarning("Invalid product ID provided: {ProductId}", productId);
                return BadRequest("Product ID cannot be empty.");
            }

            this._logger.LogDebug("Attempting to retrieve product {ProductId}", productId);
            ProductDTO? res = await this._product.TrackProductAsync(productId);

            if (res == null)
            {
                this._logger.LogError("Product {ProductId} not found or access denied", productId);
                return NotFound();
            }

            this._logger.LogInformation("Product {ProductId} retrieved successfully", productId);
            return Ok(res);
        }
    }
}
