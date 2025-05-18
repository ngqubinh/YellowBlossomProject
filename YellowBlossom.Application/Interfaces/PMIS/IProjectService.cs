using YellowBlossom.Application.DTOs.Project;
using YellowBlossom.Application.DTOs.General;
using YellowBlossom.Domain.Models.PMIS;

namespace YellowBlossom.Application.Interfaces.PMIS
{
    public interface IProjectService
    {
        Task<GeneralResponse> CreateProjectAsync(Guid productId, CreateProjectRequest model);
        Task<List<ProjectDTO>> GetProjectsAsync(Guid productId);
        Task<ProjectDTO> GetProjectById(Guid projectId);
        Task<ProjectDTO> EditProjectByIdAsync(Guid projectId, EditProjectRequest model);
        Task<ProjectDTO> AssignProjectManagerToProjectAsync(string userId, Guid projectId, string token);
        Task<string> GenerateInviteTokenAsync(Guid projectId);
        Task<ProjectTeamDTO> AssignTeamToProjectAsync(Guid projectId, AssignTeamToProjectRequest model);
        Task<GeneralResponse> DeleteProjectAsync(Guid projectId);
        ProjectDTO UpdateProjectStatus(Guid projectId, EditProjectStatusRequest request);
        Task<List<ProjectStatusDTO>> GetProjectStatusesAsync();
        Task<List<ProjectDTO>> GetProjectsRelatedToProjectManager();
    }
}
