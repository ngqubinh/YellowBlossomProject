using YellowBlossom.Application.DTOs.General;
using YellowBlossom.Application.DTOs.Setting;

namespace YellowBlossom.Application.Interfaces.Setting
{
    public interface IInitService
    {
        Task<GeneralResponse> SeedRolesAsync();
        Task<GeneralResponse> SeedProjectStatusesAsync();
        Task<GeneralResponse> SeedProjectTypesAsync();
        Task<GeneralResponse> CreateAsync(CreateInitAdminRequest model);
        Task<GeneralResponse> CreateInitAdminAsync();
    }
}
