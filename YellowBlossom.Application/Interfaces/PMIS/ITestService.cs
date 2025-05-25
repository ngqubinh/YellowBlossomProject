using YellowBlossom.Application.DTOs.General;
using YellowBlossom.Application.DTOs.Test;
using YellowBlossom.Domain.Models.PMIS;

namespace YellowBlossom.Application.Interfaces.PMIS
{
    public interface ITestService
    {
        //Task<TestCaseDTO> CreateTestCaseAsync(Guid teamId, Guid taskId, CreateTestCaseRequest request);
        Task<TestCaseDTO> CreateTestCaseAsync( Guid taskId, CreateTestCaseRequest request);
        Task<List<TestCaseDTO>> GetAllTestCasesViaTeamAsync();
        Task<List<TestCaseDTO>> GetAllTestCasesForProjectManagerAsync();
        Task<List<TestRunDTO>> GetAllTestRunsForProjectManagerAsync();
        //Task<TestRunDTO> CreateTestRunsAsync(Guid teamId, Guid taskId, CreateTestRunRequest request);
        Task<TestRunDTO> CreateTestRunsAsync(Guid taskId, CreateTestRunRequest request);
        Task<TestRunTestCaseDTO> UpdateTestRunTestCaseAsync(Guid testRunId, Guid testCaseId, UpdateTestRunTestCaseRequest request);
        Task<TestCaseDTO> GetTestCaseDetailsAsync(Guid testCasesId);
        Task<TestCaseDTO> EditTestCasesAsync(Guid testCasesId, EditTestCaseRequest request);
        Task<GeneralResponse> DeleteTestCaseAsync(Guid testCasesIsd);
        Task<TestRunDTO> GetTestRunDetailsAsync(Guid testRunId);
        Task<TestRunDTO> EditTestRunAsync(Guid testRunId, EditTestRunRequest request);
        Task<GeneralResponse> DeleteTestRunAsync(Guid testRunId);
        Task<TestSummaryDTO> GetTestSummaryAsync();
        Task<List<TestRunHistoryDTO>> GetTestRunHistoryAsync(Guid testRunId, Guid testCaseId);
        Task<List<TaskDTO>> GetDoneTasksAsync();
        Task<List<TestCaseDTO>> GetAllRelatedTestCasesAsync(Guid taskId);
        Task<List<TestRunDTO>> GetAllRelatedTestRunsAsync(Guid taskId);
        Task<List<TestRunStatusDTO>> GetAllTestRunStatusesAsync();
        Task<List<TestCaseStatusDTO>> GettAllTestCaseStatusesAsync();
        Task<List<TestTypeDTO>> GetAllTestTypesAsync();
        Task<List<TaskTestDTO>> GetAllTasksForUpdateResultAsync();
    }
}
