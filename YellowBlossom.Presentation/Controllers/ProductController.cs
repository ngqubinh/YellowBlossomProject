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

        [HttpPost("products")]
        public async Task<ActionResult<GeneralResponse>> CreateProductAsync(CreateProductRequest model)
        {
            GeneralResponse ressult = await this._product.CreateProductAsync(model);
            if(!ModelState.IsValid || ressult.Success==false)
            {
                return BadRequest(ressult);
            }
            return Ok(ressult);
        }

        [HttpPut("products/{productId}")]
        public async Task<ActionResult<ProductDTO>> EditProductAsync(Guid productId, EditProductRequest model)
        {
            ProductDTO result = await this._product.EditProductAsync(productId, model);
            if(!ModelState.IsValid || result == null)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpGet("products/{productId}")]
        public async Task<ActionResult<ProductDTO>> TrackProductAsync(Guid productId)
        {
            
            if (productId == Guid.Empty)
            {
                return NotFound("Product ID cannot be empty.");
            }

            ProductDTO? res = await this._product.TrackProductAsync(productId);

            if (res == null)
            {
                return BadRequest(res);
            }
            return Ok(res);
        }

        [HttpGet("products")]
        public async Task<ActionResult<List<ProductDTO>>> GetProductsAsync()
        {
            List<ProductDTO> res = await this._product.GetProductsAsync();
            return Ok(res);
        }

        [HttpDelete("products/{productId}")]
        public async Task<ActionResult<GeneralResponse>> DeleteProductAsync(Guid productId)
        {
            GeneralResponse res = await this._product.DeleteProductAsync(productId);
            return Ok(res);
        }

        [HttpGet("{id}/statistics")]
        public async Task<IActionResult> GetProductStatistics(Guid id)
        {
            var stats = await _product.GetProductStatisticsAsync(id);
            return Ok(stats);
        }

        [HttpGet("statistics")]
        public async Task<IActionResult> GetAllProductsStatistics()
        {
            var stats = await _product.GetAllProductsStatisticsAsync();
            return Ok(stats);
        }

    }
}
