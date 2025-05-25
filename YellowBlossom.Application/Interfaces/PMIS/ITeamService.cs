using YellowBlossom.Application.DTOs.General;
using YellowBlossom.Application.DTOs.Team;
namespace YellowBlossom.Application.Interfaces.PMIS
{
    public interface ITeamService
    {
        Task<List<TeamDTO>> GetTeamsByProductManager();
        Task<TeamDTO> CreateTeamAsync(CreateTeamRequest model);
        Task<UserTeamDTO> AssignUserToTeamAsync(AssignTeamRequest model);
        Task<List<TeamDTO>> GetCurrentTeamsAsync();
        Task<GeneralResponse> UpdateUserRolesAsync(UpdateUserRolesRequest request);
        Task<GeneralResponse> UpdateUserLockStatusAsync(UpdateUserLockRequest request);
        Task<UserDTO> GetUserDetailsAsync(string userId);
        Task<List<UserDTO>> GetAllUsersAsync();
        Task<TeamDTO> GetTeamDetailsAsync(Guid teamId);
        Task<GeneralResponse> GenerateTeamInvitationAsync(string email, Guid teamId, int expiryDays);
        Task<GeneralResponse> AcceptTeamInvitationAsync(string token);

    }
}
