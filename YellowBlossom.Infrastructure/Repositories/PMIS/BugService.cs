using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using YellowBlossom.Application.DTOs.Bug;
using YellowBlossom.Application.DTOs.General;
using YellowBlossom.Application.Interfaces.PMIS;
using YellowBlossom.Domain.Constants;
using YellowBlossom.Domain.Models.PMIS;
using YellowBlossom.Infrastructure.Data;
using YellowBlossom.Infrastructure.Services;

namespace YellowBlossom.Infrastructure.Repositories.PMIS
{
    public class BugService : IBugService
    {

        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _http;
        private readonly ILogger<TaskService> _logger;

        public BugService(ApplicationDbContext dbContext, IHttpContextAccessor http, ILogger<TaskService> logger)
        {
            this._dbContext = dbContext;
            this._http = http;
            this._logger = logger;
        }

        public async Task<List<BugDTO>> GetListOfBugsAsync()
        {
            try
            {
                // Kiểm tra HttpContext
                if (GeneralService.IsHttpContextOrUserNull(this._http.HttpContext!))
                {
                    Console.WriteLine("HttpContext or User is null.");
                    return new List<BugDTO>();
                }

                // Lấy UserId từ HttpContext
                string? userId = GeneralService.GetUserIdFromContext(this._http.HttpContext!);
                if (string.IsNullOrEmpty(userId))
                {
                    Console.WriteLine("User ID not found in HttpContext.");
                    return new List<BugDTO>();
                }

                // Kiểm tra quyền hạn của user
                List<string> allowedRoles = new List<string> { StaticUserRole.QA, StaticUserRole.Tester, StaticUserRole.ProjectManager };
                if (!HasAnyRole(this._http.HttpContext!, allowedRoles))
                {
                    Console.WriteLine("User does not have permission to get list of bugs.");
                    return new List<BugDTO>();
                }

                // Truy vấn danh sách lỗi, tối ưu bằng AsNoTracking()
                List<PMIS_Bug> bugList = await _dbContext.Bugs
                    .AsNoTracking()
                    .Include(b => b.Priority)
                    .Include(b => b.AssignedToTeam)
                    .Include(b => b.ReportedByTeam)
                    .Include(b => b.TestCase)
                    .Include(b => b.TestRun)
                    .ToListAsync();
                if (!bugList.Any())
                {
                    Console.WriteLine("No bugs found.");
                    return new List<BugDTO>();
                }

                List<BugDTO> bugDTOs = Mapper.MapBugToBugDTOByList(bugList);
                return bugDTOs;
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching bug list: {ex.Message}");
                return new List<BugDTO>();
            }
        }

        public async Task<BugDTO> UpdateBugResolutionAsync(Guid bugId, UpdateBugRequest request)
        {
            using var transaction = await this._dbContext.Database.BeginTransactionAsync();
            try
            {
                // Kiểm tra HttpContext
                if (GeneralService.IsHttpContextOrUserNull(this._http.HttpContext!))
                {
                    Console.WriteLine("HttpContext or User is null.");
                    return new BugDTO { Message = "HttpContext or User is null." };
                }

                // Lấy UserId từ HttpContext
                string? userId = GeneralService.GetUserIdFromContext(this._http.HttpContext!);
                if (string.IsNullOrEmpty(userId))
                {
                    Console.WriteLine("User ID not found in HttpContext.");
                    return new BugDTO { Message = "User ID not found in HttpContext." };
                }

                // Kiểm tra quyền hạn của user
                List<string> allowedRoles = new List<string> { StaticUserRole.Tester };
                if (!HasAnyRole(this._http.HttpContext!, allowedRoles))
                {
                    Console.WriteLine("User does not have permission to update this bug.");
                    return new BugDTO { Message = "User does not have permission to update this bug." };
                }

                var currentBug = await this._dbContext.Bugs
                    .Where(b => b.BugId == bugId)
                    .FirstOrDefaultAsync();
                if (currentBug == null)
                {
                    Console.WriteLine("Bug not found.");
                    return new BugDTO { Message = "But not found." };
                }

                Guid currentTeamOfUser = await this._dbContext.UserTeams
                    .Where(ut => ut.UserId == userId)
                    .Select(ut => ut.TeamId)
                    .FirstOrDefaultAsync();


                currentBug.StepsToReduce = string.IsNullOrEmpty(request.StepsToReduce) ? currentBug.StepsToReduce : request.StepsToReduce;
                currentBug.Serverity = string.IsNullOrEmpty(request.Serverity) ? currentBug.Serverity : request.Serverity;
                currentBug.ResolvedDate = DateTime.UtcNow;
                if (currentTeamOfUser != Guid.Empty && currentTeamOfUser != Guid.Empty)
                {
                    currentBug.AssignedToTeamId = currentTeamOfUser;
                }
                await this._dbContext.SaveChangesAsync();

                var testRunTestCase = await this._dbContext.TestRunTestCases
                    .Where(trtc => trtc.TestCaseId == currentBug.TestCaseId && trtc.TestRunId == currentBug.TestRunId)
                    .FirstOrDefaultAsync();

                if(testRunTestCase != null)
                {
                    Guid resetStatusId = await this._dbContext.TestCaseStatuses
                        .Where(tcs => tcs .TestCaseStatusName == StaticTestCaseStatus.Retest)
                        .Select(tcs => tcs.TestCaseStatusId)
                        .FirstOrDefaultAsync();

                    testRunTestCase.TestCaseStatusId = resetStatusId;
                    await this._dbContext.SaveChangesAsync();
                }

                await transaction.CommitAsync();

                BugDTO bugDTO = Mapper.MapBugToBugDTO(currentBug);
                return bugDTO;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<GeneralResponse> DeleteBugWithoutTestCaseAsync(Guid bugId)
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
                List<string> allowedRoles = new List<string> { StaticUserRole.QA, StaticUserRole.Tester, StaticUserRole.ProjectManager };
                if (!HasAnyRole(this._http.HttpContext!, allowedRoles))
                {
                    Console.WriteLine("User does not have permission to delete this bug.");
                    return new GeneralResponse(false, "User does not have permission to delete this bug.");
                }

                PMIS_Bug? bug = await this._dbContext.Bugs
                    .Where(b => b.BugId == bugId && b.TestCaseId == Guid.Empty)
                    .FirstOrDefaultAsync();
                if (bug == null)
                {
                    Console.WriteLine("Bug not found or linked to a test case.");
                    return new GeneralResponse(false, "Bug not found or linked to a test case.");
                }

                this._dbContext.Bugs.Remove(bug);
                await this._dbContext.SaveChangesAsync();
                return new GeneralResponse(true, "Bug deleted successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting bug: {ex.Message}");
                return new GeneralResponse(false, "Failed to delete bug.");
            }
        }

        public async Task<List<BugDTO>> GetBugsByTeamAsync()
        {
            try
            {
                // Kiểm tra HttpContext
                if (GeneralService.IsHttpContextOrUserNull(this._http.HttpContext!))
                {
                    Console.WriteLine("HttpContext or User is null.");
                    return new List<BugDTO>();
                }

                // Lấy UserId từ HttpContext
                string? userId = GeneralService.GetUserIdFromContext(this._http.HttpContext!);
                if (string.IsNullOrEmpty(userId))
                {
                    Console.WriteLine("User ID not found in HttpContext.");
                    return new List<BugDTO>();
                }

                // Kiểm tra quyền hạn của user
                List<string> allowedRoles = new List<string> { StaticUserRole.QA, StaticUserRole.Tester };
                if (!HasAnyRole(this._http.HttpContext!, allowedRoles))
                {
                    Console.WriteLine("User does not have permission to get list of bugs.");
                    return new List<BugDTO>();
                }

                Guid userTeamId = await this._dbContext.UserTeams
                    .Where(ut => ut.UserId == userId)
                    .Select(ut => ut.TeamId)
                    .FirstOrDefaultAsync();
                if (userTeamId == Guid.Empty)
                {
                    return new List<BugDTO>();
                }

                List<PMIS_Bug> bugs = await this._dbContext.Bugs
                    .Include(b => b.Priority)
                    .Include(b => b.TestRun)
                    .Include(b => b.TestCase)
                    .Include(b => b.ReportedByTeam)
                    .Include(b => b.AssignedToTeam)
                    .Where(b => b.AssignedToTeamId == userTeamId)
                    .ToListAsync();

                List<BugDTO> bugDTOs = Mapper.MapBugToBugDTOByList(bugs);
                return bugDTOs;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching bugs by team: {ex.Message}");
                return new List<BugDTO>();
            }
        }

        public async Task<BugStatisticsDTO> GetBugStatisticsAsync()
        {
            try
            {
                // Kiểm tra HttpContext
                if (GeneralService.IsHttpContextOrUserNull(this._http.HttpContext!))
                {
                    Console.WriteLine("HttpContext or User is null.");
                    return new BugStatisticsDTO();
                }

                // Lấy UserId từ HttpContext
                string? userId = GeneralService.GetUserIdFromContext(this._http.HttpContext!);
                if (string.IsNullOrEmpty(userId))
                {
                    Console.WriteLine("User ID not found in HttpContext.");
                    return new BugStatisticsDTO();
                }

                // Kiểm tra quyền hạn của user
                List<string> allowedRoles = new List<string> { StaticUserRole.QA, StaticUserRole.Tester, StaticUserRole.ProjectManager };
                if (!HasAnyRole(this._http.HttpContext!, allowedRoles))
                {
                    Console.WriteLine("User does not have permission to get statistics.");
                    return new BugStatisticsDTO();
                }

                var bugStatusStats = await this._dbContext.Bugs
                    .GroupBy(b => b.ResolvedDate == null ? "Unresolved" : "Resolved")
                    .Select(g => new BugStatusCountDTO
                    {
                        Status = g.Key,
                        Count = g.Count()
                    }).ToListAsync();

                var severityStats = await this._dbContext.Bugs
                    .GroupBy(b => b.Serverity)
                    .Select(g => new BugSeverityCountDTO
                    {
                        Severity = g.Key,
                        Count = g.Count()
                    }).ToListAsync();

                var priorityStats = await _dbContext.Bugs
                    .Include(b => b.Priority)
                    .GroupBy(b => b.Priority!.PriorityName)
                    .Select(g => new BugPriorityCountDTO
                    {
                        Priority = g.Key,
                        Count = g.Count()
                    })
                    .ToListAsync();

                var teamStats = await _dbContext.Bugs
                  .Include(b => b.ReportedByTeam)
                  .GroupBy(b => b.ReportedByTeam!.TeamName)
                  .Select(g => new BugTeamCountDTO
                  {
                      TeamName = g.Key,
                      Count = g.Count()
                  })
                  .ToListAsync();

                return new BugStatisticsDTO
                {
                    BugStatusStats = bugStatusStats,
                    SeverityStats = severityStats,
                    PriorityStats = priorityStats,
                    TeamStats = teamStats
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching bug statistics: {ex.Message}");
                return new BugStatisticsDTO();
            }
        }

        public async Task<BugDTO> GetBugDetailsAsync(Guid bugId)
        {
            try
            {
                // Kiểm tra HttpContext
                if (GeneralService.IsHttpContextOrUserNull(this._http.HttpContext!))
                {
                    Console.WriteLine("HttpContext or User is null.");
                    return new BugDTO { Message = "HttpContext or User is null." };
                }

                // Lấy UserId từ HttpContext
                string? userId = GeneralService.GetUserIdFromContext(this._http.HttpContext!);
                if (string.IsNullOrEmpty(userId))
                {
                    Console.WriteLine("User ID not found in HttpContext.");
                    return new BugDTO { Message = "User ID not found in HttpContext." };
                }

                // Kiểm tra quyền hạn của user
                List<string> allowedRoles = new List<string> { StaticUserRole.Tester };
                if (!HasAnyRole(this._http.HttpContext!, allowedRoles))
                {
                    Console.WriteLine("User does not have permission to get this bug.");
                    return new BugDTO { Message = "User does not have permission to get this bug." };
                }

                PMIS_Bug? bug = await this._dbContext.Bugs
                    .Include(b => b.Priority)
                    .Include(b => b.TestCase)
                    .Include(b => b.TestRun)
                    .Include(b => b.ReportedByTeam)
                    .Include(b => b.AssignedToTeam)
                    .Where(b => b.BugId == bugId)
                    .FirstOrDefaultAsync();
                if (bug == null)
                {
                    Console.WriteLine("Bug not found.");
                    return new BugDTO { Message = "Bug not found." };
                }

                BugDTO bugDTO = Mapper.MapBugToBugDTO(bug);
                return bugDTO;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new BugDTO { Message = ex.Message };
            }
        }

        public async Task<List<BugDTO>> GetBugsByTestRunAsync(Guid testRunId)
        {
            try
            {
                // Kiểm tra HttpContext
                if (GeneralService.IsHttpContextOrUserNull(this._http.HttpContext!))
                {
                    Console.WriteLine("HttpContext or User is null.");
                    return new List<BugDTO>();
                }

                // Lấy UserId từ HttpContext
                string? userId = GeneralService.GetUserIdFromContext(this._http.HttpContext!);
                if (string.IsNullOrEmpty(userId))
                {
                    Console.WriteLine("User ID not found in HttpContext.");
                    return new List<BugDTO>();
                }

                // Kiểm tra quyền hạn của user
                List<string> allowedRoles = new List<string> { StaticUserRole.QA, StaticUserRole.Tester };
                if (!HasAnyRole(this._http.HttpContext!, allowedRoles))
                {
                    Console.WriteLine("User does not have permission to get list of bugs.");
                    return new List<BugDTO>();
                }

                List<PMIS_Bug> bugs = await this._dbContext.Bugs
                   .Include(b => b.Priority)
                   .Include(b => b.TestRun)
                   .Include(b => b.TestCase)
                   .Include(b => b.ReportedByTeam)
                   .Include(b => b.AssignedToTeam)
                   .Where(b => b.TestRunId == testRunId)
                   .ToListAsync();

                List<BugDTO> bugDTOs = Mapper.MapBugToBugDTOByList(bugs);
                return bugDTOs;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching bugs by TestRun: {ex.Message}");
                return new List<BugDTO>();
            }
        }

        public async Task<BugDTO> EditBugAsync(Guid bugId, EditBugRequest request)
        {
            try
            {
                // Kiểm tra HttpContext
                if (GeneralService.IsHttpContextOrUserNull(this._http.HttpContext!))
                {
                    Console.WriteLine("HttpContext or User is null.");
                    return new BugDTO { Message = "HttpContext or User is null." };
                }

                // Lấy UserId từ HttpContext
                string? userId = GeneralService.GetUserIdFromContext(this._http.HttpContext!);
                if (string.IsNullOrEmpty(userId))
                {
                    Console.WriteLine("User ID not found in HttpContext.");
                    return new BugDTO { Message = "User ID not found in HttpContext." };
                }

                // Kiểm tra quyền hạn của user
                List<string> allowedRoles = new List<string> { StaticUserRole.QA };
                if (!HasAnyRole(this._http.HttpContext!, allowedRoles))
                {
                    Console.WriteLine("User does not have permission to update this bug.");
                    return new BugDTO { Message = "User does not have permission to update this bug." };
                }

                PMIS_Bug? currentBug = await this._dbContext.Bugs
                    .Include(b => b.Priority)
                    .Include(b => b.TestRun)
                    .Include(b => b.TestCase)
                    .Include(b => b.ReportedByTeam)
                    .Include(b => b.AssignedToTeam)
                    .Where(b => b.BugId == bugId)
                    .FirstOrDefaultAsync();
                if (currentBug == null)
                {
                    Console.WriteLine("Bug not found.");
                    return new BugDTO { Message = "Bug not found." };
                }

                var validPriorities = await this._dbContext.Priorities
                     .Select(p => new { p.PriorityId, p.PriorityName })
                     .ToListAsync();

                currentBug.Title = string.IsNullOrEmpty(request.Title) ? currentBug.Title : request.Title;
                currentBug.Description = string.IsNullOrEmpty(request.Description) ? currentBug.Description : request.Description;
                currentBug.StepsToReduce = string.IsNullOrEmpty(request.StepToReduce) ? currentBug.StepsToReduce : request.StepToReduce;
                currentBug.Serverity = string.IsNullOrEmpty(request.Severity) ? currentBug.Serverity : request.Severity;
                if( request.PriorityId != Guid.Empty)
                {
                    if(!validPriorities.Any(p => p.PriorityId == request.PriorityId))
                    {
                        Console.WriteLine($"Invalid PriorityId: {request.PriorityId}");
                        return new BugDTO { Message = "Invalid Priority Status selected." };
                    }
                    currentBug.PriorityId = request.PriorityId;
                }

                await this._dbContext.SaveChangesAsync();

                return Mapper.MapBugToBugDTO(currentBug);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new BugDTO { Message =  ex.Message };
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
