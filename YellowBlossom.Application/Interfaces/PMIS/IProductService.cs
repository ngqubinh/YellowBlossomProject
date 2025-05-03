using YellowBlossom.Application.DTOs.General;
using YellowBlossom.Application.DTOs.Product;

namespace YellowBlossom.Application.Interfaces.PMIS
{
    public interface IProductService
    {
        Task<GeneralResponse> CreateProductAsync(CreateProductRequest model);
        Task<ProductDTO> EditProductAsync(Guid productId, EditProductRequest model);
        Task<ProductDTO> TrackProductAsync(Guid productId);
        Task<List<ProductDTO>> GetProductsAsync();
        Task<GeneralResponse> DeleteProductAsync(Guid productId);
    }
}
