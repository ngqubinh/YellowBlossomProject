using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using YellowBlossom.Application.DTOs.General;
using YellowBlossom.Application.DTOs.Test;
using YellowBlossom.Application.Interfaces.PMIS;
using YellowBlossom.Domain.Constants;
using YellowBlossom.Domain.Models.PMIS;
using YellowBlossom.Infrastructure.Data;
using YellowBlossom.Infrastructure.Services;

namespace YellowBlossom.Infrastructure.Repositories.PMIS
{
    public class TestService : ITestService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _http;
        private readonly ILogger<ProductService> _logger;

        public TestService(ApplicationDbContext dbContext, IHttpContextAccessor http, ILogger<ProductService> logger)
        {
            this._dbContext = dbContext;
            this._http = http;
            this._logger = logger;
        }

        public async Task<TestCaseDTO> CreateTestCaseAsync(Guid teamId, Guid taskId, CreateTestCaseRequest request)
        {
            try
            {
                if (GeneralService.IsHttpContextOrUserNull(this._http.HttpContext!))
                {
                    Console.WriteLine("HttpContext or User is null.");
                    return new TestCaseDTO { Message = "HttpContext or User is null." };
                }

                string? userId = GeneralService.GetUserIdFromContext(this._http.HttpContext!);
                if (string.IsNullOrEmpty(userId))
                {
                    Console.WriteLine("User ID not found in HttpContext.");
                    return new TestCaseDTO { Message = "User ID not found in HttpContext." };
                }

                // Kiểm tra user có thuộc team không
                bool isUserInTeam = await _dbContext.UserTeams
                    .AnyAsync(ut => ut.TeamId == teamId && ut.UserId == userId);

                if (!isUserInTeam)
                {
                    Console.WriteLine($"User {userId} is not part of Team {teamId}");
                    return new TestCaseDTO { Message = $"User does not belong to Team {teamId}." };
                }

                List<string> allowedRoles = new List<string> { StaticUserRole.QA };
                if (!HasAnyRole(this._http.HttpContext!, allowedRoles))
                {
                    Console.WriteLine("User does not have permission to create test case.");
                    return new TestCaseDTO { Message = "User does not have permission to create test case." };
                }

                var teamExists = await this._dbContext.Teams.AnyAsync(t => t.TeamId == teamId);
                if (!teamExists)
                {
                    Console.WriteLine($"Team with ID {request.CreatedBy} does not exist.");
                    return new TestCaseDTO { Message = $"Team with ID {request.CreatedBy} does not exist." };
                }

                var task = await this._dbContext.Tasks
                    .Where(t => t.TaskId == taskId)
                    .SingleOrDefaultAsync();

                Guid defaultTestTypeId = this._dbContext.TestTypes
                    .Where(tt => tt.TestTypeName == StaticTestType.Unit)
                    .Select(tt => tt.TestTypeId)
                    .FirstOrDefault();
                if(defaultTestTypeId == Guid.Empty)
                {
                    Console.WriteLine($"This type {StaticTestType.Unit} does not exist.");
                    return new TestCaseDTO { Message = $"This type {StaticTestType.Unit} does not exist." };
                }

                Guid defaultTestCaseStatusId = this._dbContext.TestCaseStatuses
                    .Where(tcs => tcs.TestCaseStatusName == StaticTestCaseStatus.Draft)
                    .Select(tcs => tcs.TestCaseStatusId)
                    .FirstOrDefault();
                if (defaultTestCaseStatusId == Guid.Empty)
                {
                    Console.WriteLine($"This status {StaticTestCaseStatus.Draft} does not exist.");
                    return new TestCaseDTO { Message = $"This status {StaticTestCaseStatus.Draft} does not exist." };
                }

                this._logger.LogInformation("Saving test case...");
                PMIS_TestCase newTestCase = new PMIS_TestCase
                {
                    Title = request.Title,
                    Description = request.Description,
                    Steps = request.Steps,
                    ExpectedResult = request.ExpetedResult,
                    ActualResult = request.ActualResult,
                    TaskId = taskId,
                    TestTypeId = defaultTestTypeId,
                    TestCaseStatusId = defaultTestCaseStatusId,
                    CreateBy = teamId
                };
                this._dbContext.TestCases.Add(newTestCase);
                await this._dbContext.SaveChangesAsync();

                this._logger.LogInformation("Created test case successfully.");
                TestCaseDTO testCaseDTO = Mapper.MapTestCaseToTestCaseDTO(newTestCase);
                return testCaseDTO;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<List<TestCaseDTO>> GetAllTestCasesViaTeamAsync()
        {
            try
            {
                if (GeneralService.IsHttpContextOrUserNull(this._http.HttpContext!))
                {
                    Console.WriteLine("HttpContext or User is null.");
                    return new List<TestCaseDTO>();
                }

                string? userId = GeneralService.GetUserIdFromContext(this._http.HttpContext!);
                if (string.IsNullOrEmpty(userId))
                {
                    Console.WriteLine("User ID not found in HttpContext.");
                    return new List<TestCaseDTO>();
                }

                // 🔹 Lấy teamId từ bảng UserTeams
                Guid? teamId = await _dbContext.UserTeams
                    .Where(ut => ut.UserId == userId)
                    .Select(ut => ut.TeamId)
                    .FirstOrDefaultAsync();

                if (teamId == null || teamId == Guid.Empty)
                {
                    Console.WriteLine($"User {userId} is not part of any team.");
                    return new List<TestCaseDTO>();
                }

                // Kiểm tra quyền của user
                List<string> allowedRoles = new List<string> { StaticUserRole.QA, StaticUserRole.Tester };
                if (!HasAnyRole(this._http.HttpContext!, allowedRoles))
                {
                    Console.WriteLine("User does not have permission to view test cases.");
                    return new List<TestCaseDTO>();
                }

                // Kiểm tra team tồn tại
                bool teamExists = await _dbContext.Teams.AnyAsync(t => t.TeamId == teamId);
                if (!teamExists)
                {
                    Console.WriteLine($"Team with ID {teamId} does not exist.");
                    return new List<TestCaseDTO>();
                }

                // Truy vấn tất cả test cases của team
                var testCases = await _dbContext.TestCases
                    .Where(tc => tc.CreateBy == teamId)
                    .Select(tc => Mapper.MapTestCaseToTestCaseDTO(tc))
                    .ToListAsync();

                return testCases;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching test cases: {ex.Message}");
                return new List<TestCaseDTO>();
            }
        }

        public async Task<List<TestCaseDTO>> GetAllTestCasesForProjectManagerAsync()
        {
            try
            {
                if (GeneralService.IsHttpContextOrUserNull(_http.HttpContext!))
                {
                    Console.WriteLine("HttpContext or User is null.");
                    return new List<TestCaseDTO>();
                }

                string? userId = GeneralService.GetUserIdFromContext(_http.HttpContext!);
                if (string.IsNullOrEmpty(userId))
                {
                    Console.WriteLine("User ID not found in HttpContext.");
                    return new List<TestCaseDTO>();
                }

                // Kiểm tra nếu user là Project Manager
                if (!_http.HttpContext!.User.IsInRole(StaticUserRole.ProjectManager))
                {
                    _logger.LogWarning("User does not have permission to get list of test cases.");
                    return new List<TestCaseDTO>();
                }

                // Lấy tất cả test cases trong hệ thống
                var testCases = await _dbContext.TestCases
                    .Select(tc => Mapper.MapTestCaseToTestCaseDTO(tc))
                    .ToListAsync();

                return testCases;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching test cases: {ex.Message}");
                return new List<TestCaseDTO>();
            }
        }

        public async Task<List<TestRunDTO>> GetAllTestRunsForProjectManagerAsync()
        {
            try
            {
                if (GeneralService.IsHttpContextOrUserNull(_http.HttpContext!))
                {
                    Console.WriteLine("HttpContext or User is null.");
                    return new List<TestRunDTO>();
                }

                string? userId = GeneralService.GetUserIdFromContext(_http.HttpContext!);
                if (string.IsNullOrEmpty(userId))
                {
                    Console.WriteLine("User ID not found in HttpContext.");
                    return new List<TestRunDTO>();
                }

                // Kiểm tra nếu user là Project Manager
                if (!_http.HttpContext!.User.IsInRole(StaticUserRole.ProjectManager))
                {
                    _logger.LogWarning("User does not have permission to get list of test runs.");
                    return new List<TestRunDTO>();
                }

                var testRuns = await this._dbContext.TestRuns
                    .Select(tr => Mapper.MapTestRunToTestRunDTO(tr))
                    .ToListAsync();

                return testRuns;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching test runs: {ex.Message}");
                return new List<TestRunDTO>();
            }
        }

        public async Task<TestRunDTO> CreateTestRunsAsync(Guid teamId, Guid taskId, CreateTestRunRequest request)
        {
            try
            {
                if (GeneralService.IsHttpContextOrUserNull(this._http.HttpContext!))
                {
                    Console.WriteLine("HttpContext or User is null.");
                    return new TestRunDTO { Message = "HttpContext or User is null." };
                }

                string? userId = GeneralService.GetUserIdFromContext(this._http.HttpContext!);
                if (string.IsNullOrEmpty(userId))
                {
                    Console.WriteLine("User ID not found in HttpContext.");
                    return new TestRunDTO { Message = "User ID not found in HttpContext." };
                }

                // Kiểm tra user có thuộc team không
                bool isUserInTeam = await _dbContext.UserTeams
                    .AnyAsync(ut => ut.TeamId == teamId && ut.UserId == userId);

                if (!isUserInTeam)
                {
                    Console.WriteLine($"User {userId} is not part of Team {teamId}");
                    return new TestRunDTO { Message = $"User does not belong to Team {teamId}." };
                }

                List<string> allowedRoles = new List<string> { StaticUserRole.QA };
                if (!HasAnyRole(this._http.HttpContext!, allowedRoles))
                {
                    Console.WriteLine("User does not have permission to create test run.");
                    return new TestRunDTO { Message = "User does not have permission to create test run." };
                }

                var task = await this._dbContext.Tasks
                    .SingleOrDefaultAsync(t => t.TaskId == taskId);
                if (task == null)
                {
                    Console.WriteLine($"Task with ID {taskId} not found.");
                    return new TestRunDTO { Message = $"Task with ID {taskId} does not exist." };
                }

                Guid defaultTestRunStatusId = await this._dbContext.TestRunStatuses
                    .Where(trs => trs.TestRunStatusName == StaticTestRunStatus.Pending)
                    .Select(trs => trs.TestRunStatusId)
                    .FirstOrDefaultAsync();
                if(defaultTestRunStatusId == Guid.Empty)
                {
                    Console.WriteLine("Default test run status not found.");
                    return new TestRunDTO { Message = "Default test run status not found." };
                }

                PMIS_TestRun newTestRun = new PMIS_TestRun
                {
                    Title = request.Title,
                    Description = request.Description,
                    RunDate = request.RunDate,
                    TaskId = taskId,
                    CreatedBy = teamId,
                    ExecutedBy = request.ExecutedBy.HasValue ? request.ExecutedBy.Value : teamId,
                    TestRunStatusId = defaultTestRunStatusId,
                };
                this._dbContext.TestRuns.Add(newTestRun);
                await this._dbContext.SaveChangesAsync();

                Console.WriteLine("Test run created successfully!");
                TestRunDTO testRunDTO = Mapper.MapTestRunToTestRunDTO(newTestRun);
                return testRunDTO;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating test run: {ex.Message}");
                return new TestRunDTO { Message = "Internal server error." };
            }
        }

        public async Task<TestRunTestCaseDTO> UpdateTestRunTestCaseAsync(Guid testRunId, Guid testCaseId, UpdateTestRunTestCaseRequest request)
        {
            using var transaction = await this._dbContext.Database.BeginTransactionAsync();
            try
            {
                // Kiểm tra HttpContext
                if (GeneralService.IsHttpContextOrUserNull(_http.HttpContext!))
                {
                    Console.WriteLine("HttpContext or User is null.");
                    return new TestRunTestCaseDTO { Message = "HttpContext or User is null." };
                }

                // Lấy thông tin người thực hiện
                string? userId = GeneralService.GetUserIdFromContext(_http.HttpContext!);
                if (string.IsNullOrEmpty(userId))
                {
                    Console.WriteLine("User ID not found in HttpContext.");
                    return new TestRunTestCaseDTO { Message = "User ID not found in HttpContext." };
                }

                // 🔹 Nếu executedTeamId không được truyền từ request, truy vấn team của user
                Guid? executedTeamId = await _dbContext.UserTeams
                    .Where(ut => ut.UserId == userId)
                    .Select(ut => ut.TeamId)
                    .FirstOrDefaultAsync();

                if (executedTeamId == null || executedTeamId == Guid.Empty)
                {
                    Console.WriteLine($"User {userId} is not associated with any team.");
                    return new TestRunTestCaseDTO { Message = "User is not associated with any team." };
                }

                // Kiểm tra quyền hạn của user trong team
                bool isUserInTeam = await _dbContext.UserTeams.AnyAsync(ut => ut.TeamId == executedTeamId && ut.UserId == userId);
                if (!isUserInTeam)
                {
                    Console.WriteLine($"User {userId} is not part of Team {executedTeamId}");
                    return new TestRunTestCaseDTO { Message = $"User does not belong to Team {executedTeamId}." };
                }

                // Kiểm tra vai trò (QA, Tester)
                List<string> allowedRoles = new List<string> { StaticUserRole.QA, StaticUserRole.Tester };
                if (!HasAnyRole(_http.HttpContext!, allowedRoles))
                {
                    Console.WriteLine("User does not have permission to update test case.");
                    return new TestRunTestCaseDTO { Message = "User does not have permission to update test case." };
                }

                // Lấy test case cần cập nhật
                // Kiểm tra nếu test case đã được liên kết với test run
                var testRunTestCase = await _dbContext.TestRunTestCases
                    .Where(trtc => trtc.TestRunId == testRunId && trtc.TestCaseId == testCaseId)
                    .FirstOrDefaultAsync();

                bool isStatusExists = await _dbContext.TestCaseStatuses
                    .AnyAsync(ts => ts.TestCaseStatusId == request.TestCaseStatusId);

                if (!isStatusExists)
                {
                    return new TestRunTestCaseDTO { Message = "Invalid TestCaseStatusId." };
                }

                if (testRunTestCase == null)
                {
                    testRunTestCase = new PMIS_TestRunTestCase
                    {
                        TestRunId = testRunId,
                        TestCaseId = testCaseId,
                        ActualResult = "",
                        TestCaseStatusId = request.TestCaseStatusId,
                        ExecutedByTeamId = executedTeamId,
                        ExecutedAt = DateTime.UtcNow
                    };

                    _dbContext.TestRunTestCases.Add(testRunTestCase);
                    await _dbContext.SaveChangesAsync();
                }

                // Sau đó mới cập nhật kết quả test
                testRunTestCase.ActualResult = request.ActualResult;
                testRunTestCase.TestCaseStatusId = request.TestCaseStatusId;
                testRunTestCase.ExecutedByTeamId = executedTeamId.Value;
                testRunTestCase.ExecutedAt = DateTime.UtcNow;

                await _dbContext.SaveChangesAsync();

                Guid failedStatusId = await _dbContext.TestCaseStatuses
                .Where(tcs => tcs.TestCaseStatusName == StaticTestCaseStatus.Failed)
                .Select(tcs => tcs.TestCaseStatusId)
                .FirstOrDefaultAsync();

                bool isFailed = await _dbContext.TestRunTestCases
                    .Where(trtc => trtc.TestRunId == testRunId && trtc.TestCaseId == testCaseId)
                    .Select(trtc => trtc.TestCaseStatusId)
                    .FirstOrDefaultAsync() == failedStatusId;

                if (isFailed == true)
                {
                    Guid defaultPriority = this._dbContext.Priorities
                    .Where(p => p.PriorityName == StaticPriority.Medium)
                    .Select(p => p.PriorityId)
                    .FirstOrDefault();
                    if (defaultPriority == Guid.Empty)
                    {
                        Console.WriteLine($"This priority {StaticPriority.Medium} does not exist.");
                        return new TestRunTestCaseDTO { Message = $"This priority {StaticPriority.Medium} does not exist." };
                    }

                    PMIS_Bug newBug = new PMIS_Bug
                    {
                        Title = $"Bug from TestCase {testCaseId}.",
                        Description = request.ActualResult,
                        StepsToReduce = "N/A",
                        Serverity = "Servirity could not be specified.",
                        ReportedDate = DateTime.UtcNow,
                        PriorityId = defaultPriority,
                        TestRunId = testRunId,
                        TestCaseId = testCaseId,
                        ReportedByTeamId = executedTeamId.Value,
                    };
                    this._dbContext.Bugs.Add(newBug);
                    await _dbContext.SaveChangesAsync();
                }

                await transaction.CommitAsync();

                return new TestRunTestCaseDTO { Message = "Test case result updated successfully." };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Error updating test case result: {ex.Message}");
                return new TestRunTestCaseDTO { Message = "Failed to update test case result." };
            }
        }

        public async Task<TestCaseDTO> GetTestCaseDetailsAsync(Guid testCasesId)
        {
            try
            {
                if (GeneralService.IsHttpContextOrUserNull(this._http.HttpContext!))
                {
                    Console.WriteLine("HttpContext or User is null.");
                    return new TestCaseDTO { Message = "HttpContext or User is null." };
                }

                string? userId = GeneralService.GetUserIdFromContext(this._http.HttpContext!);
                if (string.IsNullOrEmpty(userId))
                {
                    Console.WriteLine("User ID not found in HttpContext.");
                    return new TestCaseDTO { Message = "User ID not found in HttpContext." };
                }

                List<string> allowedRoles = new List<string> { StaticUserRole.QA, StaticUserRole.Tester, StaticUserRole.ProjectManager };
                if (!HasAnyRole(this._http.HttpContext!, allowedRoles))
                {
                    Console.WriteLine("User does not have permission to get this test case.");
                    return new TestCaseDTO { Message = "User does not have permission to get this test case." };
                }

                PMIS_TestCase? testCase = await this._dbContext.TestCases
                    .Include(tc => tc.Task)
                    .Include(tc => tc.Team)
                    .Include(tc => tc.TestRunTestCases)
                    .Include(tc => tc.TestType)
                    .Where(tc => tc.TestCaseId == testCasesId)
                    .FirstOrDefaultAsync();
                if (testCase == null)
                {
                    Console.WriteLine("There is not test case.");
                    return new TestCaseDTO { Message = "There is not test case." };
                }

                TestCaseDTO testCaseDTO = Mapper.MapTestCaseToTestCaseDTO(testCase);
                return testCaseDTO;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new TestCaseDTO { Message = ex.Message };
            }
        }

        public async Task<TestCaseDTO> EditTestCasesAsync(Guid testCasesId, EditTestCaseRequest request)
        {
            try
            {
                if (GeneralService.IsHttpContextOrUserNull(this._http.HttpContext!))
                {
                    Console.WriteLine("HttpContext or User is null.");
                    return new TestCaseDTO { Message = "HttpContext or User is null." };
                }

                string? userId = GeneralService.GetUserIdFromContext(this._http.HttpContext!);
                if (string.IsNullOrEmpty(userId))
                {
                    Console.WriteLine("User ID not found in HttpContext.");
                    return new TestCaseDTO { Message = "User ID not found in HttpContext." };
                }

                List<string> allowedRoles = new List<string> { StaticUserRole.QA };
                if (!HasAnyRole(this._http.HttpContext!, allowedRoles))
                {
                    Console.WriteLine("User does not have permission to edit this test case.");
                    return new TestCaseDTO { Message = "User does not have permission to edit this test case." };
                }

                PMIS_TestCase? currentTestCase = await this._dbContext.TestCases
                    .Where(tc => tc.TestCaseId == testCasesId)
                    .FirstOrDefaultAsync();
                if (currentTestCase == null)
                {
                    Console.WriteLine("Test case not found.");
                    return new TestCaseDTO { Message = "Test case not found." };
                }

                var validTestTypeId = this._dbContext.TestTypes
                    .Select(tt => new { tt.TestTypeId, tt.TestTypeName })
                    .ToList();

                var validTestCaseStatusId = this._dbContext.TestCaseStatuses
                    .Select(tcs => new { tcs.TestCaseStatusId, tcs.TestCaseStatusName })
                    .ToList();

                currentTestCase.Title = string.IsNullOrEmpty(request.Title) ? currentTestCase.Title : request.Title;
                currentTestCase.Description = string.IsNullOrEmpty(request.Description) ? currentTestCase.Description : request.Description;
                currentTestCase.Steps = string.IsNullOrEmpty(request.Steps) ? currentTestCase.Steps : request.Steps;
                currentTestCase.ExpectedResult = string.IsNullOrEmpty(request.ExpectedResult) ? currentTestCase.ExpectedResult : request.ExpectedResult;
                currentTestCase.ActualResult = string.IsNullOrEmpty(request.ActualResult) ? currentTestCase.ActualResult : request.ActualResult;
                if( request.TestTypeId != Guid.Empty)
                {
                    if(!validTestTypeId.Any(tt => tt.TestTypeId == currentTestCase.TestTypeId))
                    {
                        Console.WriteLine($"Invalid TestTypeId: {request.TestTypeId}");
                        return new TestCaseDTO { Message = "Invalid Test Type selected." };
                    }
                    currentTestCase.TestTypeId = request.TestTypeId;
                }
                if (request.TestCaseStatusId != Guid.Empty)
                {
                    if (!validTestCaseStatusId.Any(tt => tt.TestCaseStatusId == currentTestCase.TestCaseStatusId))
                    {
                        Console.WriteLine($"Invalid TestCaseStatusId: {request.TestCaseStatusId}");
                        return new TestCaseDTO { Message = "Invalid Test Case Status selected." };
                    }
                    currentTestCase.TestCaseStatusId = request.TestCaseStatusId;
                }
                await this._dbContext.SaveChangesAsync();

                TestCaseDTO testCaseDTO = Mapper.MapTestCaseToTestCaseDTO(currentTestCase);
                return testCaseDTO;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new TestCaseDTO { Message = ex.Message };
            }
        }

        public async Task<GeneralResponse> DeleteTestCaseAsync(Guid testCasesId)
        {
            try
            {
                // Kiểm tra HttpContext
                if (GeneralService.IsHttpContextOrUserNull(this._http.HttpContext!))
                {
                    Console.WriteLine("HttpContext or User is null.");
                    return new GeneralResponse(false, "HttpContext or Uesr is null.");
                }

                // Lấy UserId từ HttpContext
                string? userId = GeneralService.GetUserIdFromContext(this._http.HttpContext!);
                if (string.IsNullOrEmpty(userId))
                {
                    Console.WriteLine("User ID not found in HttpContext.");
                    return new GeneralResponse(false, "User ID not found in HttpContext.");
                }

                // Kiểm tra quyền hạn của user
                List<string> allowedRoles = new List<string> { StaticUserRole.QA };
                if (!HasAnyRole(this._http.HttpContext!, allowedRoles))
                {
                    Console.WriteLine("User does not have permission to delete this test case.");
                    return new GeneralResponse(false, "User does not have permission to delete this test case.");
                }

                PMIS_TestCase? testCase = await this._dbContext.TestCases
                    .Where(tc => tc.TestCaseId == testCasesId)
                    .FirstOrDefaultAsync();
                if (testCase == null)
                {
                    Console.WriteLine("Test cae not found.");
                    return new GeneralResponse(false, "Test case not found.");
                }

                this._dbContext.TestCases.Remove(testCase);
                await this._dbContext.SaveChangesAsync();
                return new GeneralResponse(true, "Delete test case successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new GeneralResponse(false, ex.Message);
            }
        }

        public async Task<TestRunDTO> GetTestRunDetailsAsync(Guid testRunId)
        {
            try
            {
                if (GeneralService.IsHttpContextOrUserNull(this._http.HttpContext!))
                {
                    Console.WriteLine("HttpContext or User is null.");
                    return new TestRunDTO { Message = "HttpContext or User is null." };
                }

                string? userId = GeneralService.GetUserIdFromContext(this._http.HttpContext!);
                if (string.IsNullOrEmpty(userId))
                {
                    Console.WriteLine("User ID not found in HttpContext.");
                    return new TestRunDTO { Message = "User ID not found in HttpContext." };
                }

                List<string> allowedRoles = new List<string> { StaticUserRole.QA, StaticUserRole.Tester, StaticUserRole.ProjectManager };
                if (!HasAnyRole(this._http.HttpContext!, allowedRoles))
                {
                    Console.WriteLine("User does not have permission to get this test run.");
                    return new TestRunDTO { Message = "User does not have permission to get this test run." };
                }

                PMIS_TestRun? testRun = await this._dbContext.TestRuns
                   .Include(tc => tc.Task)
                   .Include(tc => tc.CreatedByTeam)
                   .Include(tc => tc.ExecutedByTeam)
                   .Include(tc => tc.TestRunStatus)
                   .Where(tc => tc.TestRunId == testRunId)
                   .FirstOrDefaultAsync();
                if (testRun == null)
                {
                    Console.WriteLine("There is not test run.");
                    return new TestRunDTO { Message = "There is not test run." };
                }

                TestRunDTO testRunDTO = Mapper.MapTestRunToTestRunDTO(testRun);
                return testRunDTO;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new TestRunDTO { Message = ex.Message };
            }
        }

        public async Task<TestRunDTO> EditTestRunAsync(Guid testRunId, EditTestRunRequest request)
        {
            try
            {
                if (GeneralService.IsHttpContextOrUserNull(this._http.HttpContext!))
                {
                    Console.WriteLine("HttpContext or User is null.");
                    return new TestRunDTO { Message = "HttpContext or User is null." };
                }

                string? userId = GeneralService.GetUserIdFromContext(this._http.HttpContext!);
                if (string.IsNullOrEmpty(userId))
                {
                    Console.WriteLine("User ID not found in HttpContext.");
                    return new TestRunDTO { Message = "User ID not found in HttpContext." };
                }

                List<string> allowedRoles = new List<string> { StaticUserRole.QA };
                if (!HasAnyRole(this._http.HttpContext!, allowedRoles))
                {
                    Console.WriteLine("User does not have permission to edit this test run.");
                    return new TestRunDTO { Message = "User does not have permission to edit this test run." };
                }

                PMIS_TestRun? testRun = await this._dbContext.TestRuns
                    .Where(tr => tr.TestRunId == testRunId)
                    .FirstOrDefaultAsync();
                if (testRun == null)
                {
                    Console.WriteLine("Test run not found.");
                    return new TestRunDTO { Message = "Test run not found." };
                }

                var validTestRunStatusId = this._dbContext.TestRunStatuses
                    .Select(trs => new { trs.TestRunStatusId, trs.TestRunStatusName })
                    .ToList();

                testRun.Title = string.IsNullOrEmpty(request.Title) ? testRun.Title : request.Title;
                testRun.Description = string.IsNullOrEmpty(request.Description) ? testRun.Description : request.Description;
                if(request.TestRunStatusId != Guid.Empty)
                {
                    if(!validTestRunStatusId.Any(trs => trs.TestRunStatusId == request.TestRunStatusId))
                    {
                        Console.WriteLine($"Invalid TestRunStatusId: {request.TestRunStatusId}");
                        return new TestRunDTO { Message = "Invalid Test Run Status selected." };
                    }
                    testRun.TestRunStatusId = request.TestRunStatusId;
                }

                await this._dbContext.SaveChangesAsync();
                TestRunDTO testRunDTO = Mapper.MapTestRunToTestRunDTO(testRun);
                return testRunDTO;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new TestRunDTO { Message = ex.Message };
            }
        }

        public async Task<GeneralResponse> DeleteTestRunAsync(Guid testRunId)
        {
            try
            {
                // Kiểm tra HttpContext
                if (GeneralService.IsHttpContextOrUserNull(this._http.HttpContext!))
                {
                    Console.WriteLine("HttpContext or User is null.");
                    return new GeneralResponse(false, "HttpContext or Uesr is null.");
                }

                // Lấy UserId từ HttpContext
                string? userId = GeneralService.GetUserIdFromContext(this._http.HttpContext!);
                if (string.IsNullOrEmpty(userId))
                {
                    Console.WriteLine("User ID not found in HttpContext.");
                    return new GeneralResponse(false, "User ID not found in HttpContext.");
                }

                // Kiểm tra quyền hạn của user
                List<string> allowedRoles = new List<string> { StaticUserRole.QA };
                if (!HasAnyRole(this._http.HttpContext!, allowedRoles))
                {
                    Console.WriteLine("User does not have permission to delete this test run.");
                    return new GeneralResponse(false, "User does not have permission to delete this test run.");
                }

                PMIS_TestRun? testRun = await this._dbContext.TestRuns
                    .Where(tr => tr.TestRunId == testRunId)
                    .FirstOrDefaultAsync();
                if (testRun == null)
                {
                    Console.WriteLine("Test run not found.");
                    return new GeneralResponse(false, "Test run not found.");
                }

                this._dbContext.TestRuns.Remove(testRun);
                await this._dbContext.SaveChangesAsync();
                return new GeneralResponse(true, "Delete test run succesfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new GeneralResponse(false, ex.Message);
            }
        }

        public async Task<TestSummaryDTO> GetTestSummaryAsync()
        {
            try
            {
                if (GeneralService.IsHttpContextOrUserNull(this._http.HttpContext!))
                {
                    Console.WriteLine("HttpContext or User is null.");
                    return new TestSummaryDTO { Message = "HttpContext or User is null." };
                }

                string? userId = GeneralService.GetUserIdFromContext(this._http.HttpContext!);
                if (string.IsNullOrEmpty(userId))
                {
                    Console.WriteLine("User ID not found in HttpContext.");
                    return new TestSummaryDTO { Message = "User ID not found in HttpContext." };
                }

                List<string> allowedRoles = new List<string> { StaticUserRole.QA, StaticUserRole.Tester, StaticUserRole.ProjectManager };
                if (!HasAnyRole(this._http.HttpContext!, allowedRoles))
                {
                    Console.WriteLine("User does not have permission to get the statistics.");
                    return new TestSummaryDTO { Message = "User does not have permission to get the statistics." };
                }

                int totalTestCases = await this._dbContext.TestCases.CountAsync();
                int totalTestRuns = await this._dbContext.TestRuns.CountAsync();
                int executedTestCases = await this._dbContext.TestRunTestCases
                    .Where(trtc => trtc.ExecutedAt != DateTime.MinValue)
                    .CountAsync();
                int failedTestCases = await this._dbContext.TestRunTestCases
                    .Include(trtc => trtc.TestCaseStatus)
                    .Where(trtc => trtc.TestCaseStatus!.TestCaseStatusName == StaticTestCaseStatus.Failed)
                    .CountAsync();
                return new TestSummaryDTO
                {
                    TotalTestCases = totalTestCases,
                    TotalTestRuns = totalTestRuns,
                    ExecutedTestCases = executedTestCases,
                    FailedTestCases = failedTestCases
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching test summary: {ex.Message}");
                return new TestSummaryDTO();
            }
        }

        public async Task<List<TestRunHistoryDTO>> GetTestRunHistoryAsync(Guid testRunId, Guid testCaseId)
        {
            try
            {
                if (GeneralService.IsHttpContextOrUserNull(this._http.HttpContext!))
                {
                    Console.WriteLine("HttpContext or User is null.");
                    return new List<TestRunHistoryDTO>();
                }

                string? userId = GeneralService.GetUserIdFromContext(this._http.HttpContext!);
                if (string.IsNullOrEmpty(userId))
                {
                    Console.WriteLine("User ID not found in HttpContext.");
                    return new List<TestRunHistoryDTO>();
                }

                List<string> allowedRoles = new List<string> { StaticUserRole.ProjectManager, StaticUserRole.ADMIN };
                if (!HasAnyRole(this._http.HttpContext!, allowedRoles))
                {
                    Console.WriteLine("User does not have permission to get the statistics.");
                    return new List<TestRunHistoryDTO>();
                }

                var history = await _dbContext.TestRunTestCases
                    .Where(trtc => trtc.TestRunId == testRunId && trtc.TestCaseId == testCaseId)
                    .OrderBy(trtc => trtc.ExecutedAt)
                    .Select(trtc => new TestRunHistoryDTO
                    {
                        TestRunId = trtc.TestRunId,
                        TestCaseId = trtc.TestCaseId,
                        ActualResult = trtc.ActualResult ?? "No result recorded",
                        TestCaseStatus = trtc.TestCaseStatus!.TestCaseStatusName,
                        ExecutedByTeam = trtc.ExecutedByTeam!.TeamName,
                        ExecutedAt = trtc.ExecutedAt
                    })
                    .ToListAsync();

                if (!history.Any())
                {
                    Console.WriteLine("No test run history found.");
                    return new List<TestRunHistoryDTO>();
                }

                return history;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching test run history: {ex.Message}");
                return new List<TestRunHistoryDTO>();
            }
        }

        #region extra functions
        private bool HasAnyRole(HttpContext context, List<string> roles)
        {
            ClaimsPrincipal user = context!.User;
            return roles.Any(role => user.IsInRole(role));
        }
        #endregion
    }
}
