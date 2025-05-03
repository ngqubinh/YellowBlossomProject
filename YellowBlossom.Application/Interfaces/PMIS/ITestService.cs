using YellowBlossom.Application.DTOs.General;
using YellowBlossom.Application.DTOs.Test;

namespace YellowBlossom.Application.Interfaces.PMIS
{
    public interface ITestService
    {
        Task<TestCaseDTO> CreateTestCaseAsync(Guid teamId, Guid taskId, CreateTestCaseRequest request);
        Task<List<TestCaseDTO>> GetAllTestCasesViaTeamAsync();
        Task<List<TestCaseDTO>> GetAllTestCasesForProjectManagerAsync();
        Task<List<TestRunDTO>> GetAllTestRunsForProjectManagerAsync();
        Task<TestRunDTO> CreateTestRunsAsync(Guid teamId, Guid taskId, CreateTestRunRequest request);
        Task<TestRunTestCaseDTO> UpdateTestRunTestCaseAsync(Guid testRunId, Guid testCaseId, UpdateTestRunTestCaseRequest request);
        Task<TestCaseDTO> GetTestCaseDetailsAsync(Guid testCasesId);
        Task<TestCaseDTO> EditTestCasesAsync(Guid testCasesId, EditTestCaseRequest request);
        Task<GeneralResponse> DeleteTestCaseAsync(Guid testCasesIsd);
        Task<TestRunDTO> GetTestRunDetailsAsync(Guid testRunId);
        Task<TestRunDTO> EditTestRunAsync(Guid testRunId, EditTestRunRequest request);
        Task<GeneralResponse> DeleteTestRunAsync(Guid testRunId);
        Task<TestSummaryDTO> GetTestSummaryAsync();
        Task<List<TestRunHistoryDTO>> GetTestRunHistoryAsync(Guid testRunId, Guid testCaseId);
    }
}
