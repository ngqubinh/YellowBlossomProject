using YellowBlossom.Application.DTOs.Bug;
using YellowBlossom.Application.DTOs.General;

namespace YellowBlossom.Application.Interfaces.PMIS
{
    public interface IBugService
    {
        Task<List<BugDTO>> GetListOfBugsAsync();
        Task<BugDTO> UpdateBugResolutionAsync(Guid bugId, UpdateBugRequest request);
        Task<GeneralResponse> DeleteBugWithoutTestCaseAsync(Guid bugId);
        Task<List<BugDTO>> GetBugsByTeamAsync();
        Task<BugStatisticsDTO> GetBugStatisticsAsync();
        Task<BugDTO> GetBugDetailsAsync(Guid bsugId);
        Task<List<BugDTO>> GetBugsByTestRunAsync(Guid testRunId);
        Task<BugDTO> EditBugAsync(Guid bugId, EditBugRequest request);
    }
}
