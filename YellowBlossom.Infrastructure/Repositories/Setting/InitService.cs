using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using YellowBlossom.Application.DTOs.General;
using YellowBlossom.Application.DTOs.Setting;
using YellowBlossom.Application.Interfaces.Setting;
using YellowBlossom.Domain.Constants;
using YellowBlossom.Domain.Models.Auth;
using YellowBlossom.Domain.Models.PMIS;
using YellowBlossom.Infrastructure.Data;

namespace YellowBlossom.Infrastructure.Repositories.Setting
{
    public class InitService : IInitService
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _dbContext;

        public InitService(RoleManager<IdentityRole> roleManager, UserManager<User> userManager, ApplicationDbContext dbContext)
        {
            this._roleManager = roleManager;
            this._userManager = userManager;
            this._dbContext = dbContext;
        }

        public async Task<GeneralResponse> SeedProjectStatusesAsync()
        {
            try
            {
                var projectStatuses = new[]
                {
                    StaticProjectStatus.NotStarted,
                    StaticProjectStatus.InProgress,
                    StaticProjectStatus.OnHold,
                    StaticProjectStatus.Completed,
                    StaticProjectStatus.Cancelled,
                    StaticProjectStatus.Delayed,
                    StaticProjectStatus.AtRisk
                };

                var results = new List<string>();
                int addedCount = 0;
                foreach(string status in projectStatuses)
                {
                    bool exist = await this._dbContext.ProjectStatuses.AnyAsync(ps => ps.ProjectStatusName == status);
                    if(!exist)
                    {
                        var newStatus = new PMIS_ProjectStatus(status);
                        this._dbContext.ProjectStatuses.Add(newStatus);
                        addedCount++;
                        results.Add($"Added status: {status}");
                    }
                    else
                    {
                        results.Add($"Status already exists: {status}");
                    }
                    
                }
                if (addedCount > 0)
                {
                    await _dbContext.SaveChangesAsync();
                }
                return new GeneralResponse
                (
                    success: true,
                    message: $"Seeding completed. Added {addedCount} new statuses."
                );
            }
            catch (Exception ex)
            {
                return new GeneralResponse
                (
                    success: false,
                    message: $"Error seeding project statuses: {ex.Message}"
                );
            }
        }

        public async Task<GeneralResponse> SeedProjectTypesAsync()
        {
            try
            {
                var projectTypes = new[]
                {
                    StaticProjectType.Agile,
                    StaticProjectType.Scrum,
                    StaticProjectType.Waterfall
                };

                var results = new List<string>();
                int addedCount = 0;

                foreach (string type in projectTypes)
                {
                    bool exists = await _dbContext.ProjectTypes
                        .AnyAsync(pt => pt.ProjectTypeName == type);

                    if (!exists)
                    {
                        var newType = new PMIS_ProjectType(type);

                        _dbContext.ProjectTypes.Add(newType);
                        addedCount++;
                        results.Add($"Added type: {type}");
                    }
                    else
                    {
                        results.Add($"Type already exists: {type}");
                    }
                }

                if (addedCount > 0)
                {
                    await _dbContext.SaveChangesAsync();
                }

                return new GeneralResponse
                (
                    success: true,
                    message: $"Seeding completed. Added {addedCount} new types."
                );
            }
            catch (Exception ex)
            {
                return new GeneralResponse
                (
                    success: false,
                    message: $"Error seeding project types: {ex.Message}"
                );
            }
        }

        public async Task<GeneralResponse> SeedRolesAsync()
        {
            try
            {
                var rolesToSeed = new[]
                {
                    StaticUserRole.ADMIN,
                    StaticUserRole.ProjectManager,
                    StaticUserRole.Tester,
                    StaticUserRole.Developer,
                    StaticUserRole.QA,
                    StaticUserRole.Unknown
                };

                var results = new List<string>();

                foreach (var role in rolesToSeed)
                {
                    bool exists = await _roleManager.RoleExistsAsync(role);
                    if (exists)
                    {
                        results.Add($"{role} already exists.");
                    }
                    else
                    {
                        var createResult = await _roleManager.CreateAsync(new IdentityRole { Name = role, NormalizedName = role.ToLower() });
                        if (createResult.Succeeded)
                        {
                            results.Add($"{role} created successfully.");
                        }
                        else
                        {
                            var errors = string.Join("- ", createResult.Errors.Select(e => e.Description));
                            results.Add($"{role} failed to create: {errors}");
                        }
                    }
                }

                bool allSucceeded = results.All(r => r.Contains("created successfully") || r.Contains("already exists"));
                string message = string.Join(" ", results);
                return new GeneralResponse(allSucceeded, message);
            }
            catch (Exception)
            {
                return new GeneralResponse(false, "An error occurred while seeding roles.");
            }
        }

        public async Task<GeneralResponse> SeedTaskStatusesAsync()
        {
            try
            {
                var taskStatuses = new[]
                {
                    StaticTaskStatus.ToDo,
                    StaticTaskStatus.InProgress,
                    StaticTaskStatus.Done,
                    StaticTaskStatus.Open,
                    StaticTaskStatus.InReview,
                    StaticTaskStatus.Blocked,
                    StaticTaskStatus.Resolved,
                    StaticTaskStatus.Closed,
                    StaticTaskStatus.Reopened,
                    StaticTaskStatus.AwaitingApproval
                };

                var results = new List<string>();
                int addedCount = 0;

                foreach (string status in taskStatuses)
                {
                    bool exist = await _dbContext.TaskStatuses.AnyAsync(ts => ts.TaskStatusName == status);
                    if (!exist)
                    {
                        var newStatus = new PMIS_TaskStatus(status);
                        _dbContext.TaskStatuses.Add(newStatus);
                        addedCount++;
                        results.Add($"Added task status: {status}");
                    }
                    else
                    {
                        results.Add($"Task status already exists: {status}");
                    }
                }

                if (addedCount > 0)
                {
                    await _dbContext.SaveChangesAsync();
                }

                return new GeneralResponse
                (
                    success: true,
                    message: $"Seeding completed. Added {addedCount} new task statuses."
                );
            }
            catch (Exception ex)
            {
                return new GeneralResponse
                (
                    success: false,
                    message: $"Error seeding task statuses: {ex.Message}"
                );
            }
        }

        public async Task<GeneralResponse> SeedPrioritiesAsync()
        {
            try
            {
                var priorities = new[]
                {
                    StaticPriority.Low,
                    StaticPriority.Medium,
                    StaticPriority.High,
                    StaticPriority.Critical
                };

                var results = new List<string>();
                int addedCount = 0;

                foreach (string priority in priorities)
                {
                    bool exist = await _dbContext.Priorities.AnyAsync(p => p.PriorityName == priority);
                    if (!exist)
                    {
                        var newPriority = new PMIS_Priority(priority);
                        _dbContext.Priorities.Add(newPriority);
                        addedCount++;
                        results.Add($"Added priority: {priority}");
                    }
                    else
                    {
                        results.Add($"Priority already exists: {priority}");
                    }
                }

                if (addedCount > 0)
                {
                    await _dbContext.SaveChangesAsync();
                }

                return new GeneralResponse
                (
                    success: true,
                    message: $"Seeding completed. Added {addedCount} new priorities."
                );
            }
            catch (Exception ex)
            {
                return new GeneralResponse
                (
                    success: false,
                    message: $"Error seeding priorities: {ex.Message}"
                );
            }
        }

        public async Task<GeneralResponse> SeedTestCaseStatusesAsync()
        {
            try
            {
                var testCaseStatuses = new[]
                {
                    StaticTestCaseStatus.Draft,
                    StaticTestCaseStatus.Ready,
                    StaticTestCaseStatus.InProgress,
                    StaticTestCaseStatus.Blocked,
                    StaticTestCaseStatus.Passed,
                    StaticTestCaseStatus.Failed,
                    StaticTestCaseStatus.Retest,
                    StaticTestCaseStatus.Deprecated
                };

                var results = new List<string>();
                int addedCount = 0;

                foreach (string status in testCaseStatuses)
                {
                    bool exist = await _dbContext.TestCaseStatuses.AnyAsync(ts => ts.TestCaseStatusName == status);
                    if (!exist)
                    {
                        var newStatus = new PMIS_TestCaseStatus(status);
                        _dbContext.TestCaseStatuses.Add(newStatus);
                        addedCount++;
                        results.Add($"Added test case status: {status}");
                    }
                    else
                    {
                        results.Add($"Test case status already exists: {status}");
                    }
                }

                if (addedCount > 0)
                {
                    await _dbContext.SaveChangesAsync();
                }

                return new GeneralResponse
                (
                    success: true,
                    message: $"Seeding completed. Added {addedCount} new test case statuses."
                );
            }
            catch (Exception ex)
            {
                return new GeneralResponse
                (
                    success: false,
                    message: $"Error seeding test case statuses: {ex.Message}"
                );
            }
        }

        public async Task<GeneralResponse> SeedTestTypesAsync()
        {
            try
            {
                var testTypes = new[]
                {
                    StaticTestType.Unit,
                    StaticTestType.Integration,
                    StaticTestType.System,
                    StaticTestType.UAT,
                    StaticTestType.Performance,
                    StaticTestType.Load,
                    StaticTestType.Stress,
                    StaticTestType.Security,
                    StaticTestType.Usability,
                    StaticTestType.Regression,
                    StaticTestType.Smoke,
                    StaticTestType.Exploratory,
                    StaticTestType.AdHoc
                };

                var results = new List<string>();
                int addedCount = 0;

                foreach (string type in testTypes)
                {
                    bool exist = await _dbContext.TestTypes.AnyAsync(tt => tt.TestTypeName == type);
                    if (!exist)
                    {
                        var newTestType = new PMIS_TestType(type);
                        _dbContext.TestTypes.Add(newTestType);
                        addedCount++;
                        results.Add($"Added test type: {type}");
                    }
                    else
                    {
                        results.Add($"Test type already exists: {type}");
                    }
                }

                if (addedCount > 0)
                {
                    await _dbContext.SaveChangesAsync();
                }

                return new GeneralResponse
                (
                    success: true,
                    message: $"Seeding completed. Added {addedCount} new test types."
                );
            }
            catch (Exception ex)
            {
                return new GeneralResponse
                (
                    success: false,
                    message: $"Error seeding test types: {ex.Message}"
                );
            }
        }

        public async Task<GeneralResponse> SeedTestRunStatusesAsync()
        {
            try
            {
                var testRunStatuses = new[]
                {
                    StaticTestRunStatus.Pending,
                    StaticTestRunStatus.InProgress,
                    StaticTestRunStatus.Completed,
                    StaticTestRunStatus.Failed
                };

                var results = new List<string>();
                int addedCount = 0;

                foreach (string status in testRunStatuses)
                {
                    bool exist = await _dbContext.TestRunStatuses.AnyAsync(trs => trs.TestRunStatusName == status);
                    if (!exist)
                    {
                        var newTestRunStatus = new PMIS_TestRunStatus { TestRunStatusName = status };
                        _dbContext.TestRunStatuses.Add(newTestRunStatus);
                        addedCount++;
                        results.Add($"Added test run status: {status}");
                    }
                    else
                    {
                        results.Add($"Test run status already exists: {status}");
                    }
                }

                if (addedCount > 0)
                {
                    await _dbContext.SaveChangesAsync();
                }

                return new GeneralResponse
                (
                    success: true,
                    message: $"Seeding completed. Added {addedCount} new test run statuses."
                );
            }
            catch (Exception ex)
            {
                return new GeneralResponse
                (
                    success: false,
                    message: $"Error seeding test run statuses: {ex.Message}"
                );
            }
        }

        public async Task<GeneralResponse> CreateAsync(CreateInitAdminRequest model)
        {
            if (string.IsNullOrEmpty(model.Password) || string.IsNullOrEmpty(model.Password))
                return new GeneralResponse(false, "Email or password cannot be empty.");

            try
            {
                if (await FindUserByEmailAsync(model.Email) != null)
                    return new GeneralResponse(false, $"User with email {model.Email} already exists.");

                var newUser = new User
                {
                    UserName = model.Email,
                    FullName = model.FullName,
                    Email = model.Email,
                    EmailConfirmed = true,
                };

                var createResult = await _userManager.CreateAsync(newUser, model.Password);
                if (!createResult.Succeeded)
                    return new GeneralResponse(false, FormatErrors(createResult.Errors));

                var role = new IdentityRole() { Name = model.Role };
                var assignRole = await _userManager.AddToRoleAsync(newUser, role.ToString());
                string err = CheckResponse(assignRole);
                if (!string.IsNullOrEmpty(err))
                    return new GeneralResponse(false, err);

                return new GeneralResponse(true, "User registered successfully.");
            }
            catch (Exception ex)
            {
                return new GeneralResponse(false, ex.Message.ToString());
            }
        }

        public async Task<GeneralResponse> CreateInitAdminAsync()
        {
            try
            {
                var initAdmin = new CreateInitAdminRequest
                {
                    Email = "admin@gmail.com",
                    Password = "nguyenquocbinh214@BB",
                    FullName = "Nguyễn Quốc Bình",
                    Role = StaticUserRole.ADMIN
                };

                var result = await CreateAsync(initAdmin);
                return result.Success
                    ? new GeneralResponse(true, "Initial admin created successfully.")
                    : new GeneralResponse(false, $"Failed to create initial admin: {result.Message}");
            }
            catch (Exception ex)
            {
                return new GeneralResponse(false, $"An error occurred while initializing the admin. Error: {ex.Message.ToString()}");
            }
        }

        #region extra functions
        private async Task<User?> FindUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        private static string FormatErrors(IEnumerable<IdentityError> errors)
        {
            return string.Join(", ", errors.Select(e => e.Description));
        }

        private static string CheckResponse(IdentityResult result)
        {
            if (!result.Succeeded)
            {
                var err = result.Errors.Select(i => i.Description);
                return string.Join(Environment.NewLine, err);
            }

            return null!;
        }
        #endregion
    }
}
