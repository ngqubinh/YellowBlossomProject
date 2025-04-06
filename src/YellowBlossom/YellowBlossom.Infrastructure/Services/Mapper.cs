using YellowBlossom.Application.DTOs.General;
using YellowBlossom.Domain.Models.Auth;
using YellowBlossom.Domain.Models.PMIS;

namespace YellowBlossom.Infrastructure.Services
{
    public static class Mapper
    {
        public static ProductDTO MapProductToProductDTO(Product product)
        {
            return new ProductDTO
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                Description = product.Description,
                Version = product.Version,
                CreatedBy = product.CreatedBy,
                User = product.User != null ? MapUserToUserDTO(product.User) : null,
                CreatedAt = product.CreatedAt,
                LastUpdated = product.LastUpdated
            };
        }

        public static UserDTO MapUserToUserDTO(User user)
        {
            return new UserDTO
            {
                Email = user.Email!,
                FullName = user.FullName,
            };
        }
    }
}
