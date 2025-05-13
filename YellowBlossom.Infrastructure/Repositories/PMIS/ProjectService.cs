using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using YellowBlossom.Application.DTOs.General;
using YellowBlossom.Application.DTOs.Project;
using YellowBlossom.Application.Interfaces.PMIS;
using YellowBlossom.Domain.Constants;
using YellowBlossom.Domain.Models.PMIS;
using YellowBlossom.Infrastructure.Data;
using YellowBlossom.Infrastructure.Services;

namespace YellowBlossom.Infrastructure.Repositories.PMIS
{
    public class ProjectService : IProjectService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _http;
        private readonly ILogger<ProjectService> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ProjectService(ApplicationDbContext dbContext, IHttpContextAccessor http, ILogger<ProjectService> logger, RoleManager<IdentityRole> roleManager)
        {
            this._dbContext = dbContext;
            this._http = http;
            this._logger = logger;
            this._roleManager = roleManager;
            LogContext.PushProperty("ServiceName", "ProjectService");
        }

        public async Task<GeneralResponse> CreateProjectAsync(Guid productId, CreateProjectRequest model)
        {
            try
            {
                if(this._http.HttpContext?.User == null)
                {
                    Console.WriteLine("HttpContext or User is null.");
                    return new GeneralResponse(false, "User authentication failed.");
                }

                string? userId = this._http.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    Console.WriteLine("User ID not found in HttpContext.");
                    return new GeneralResponse(false, "User not authenticated.");
                }


                List<string> allowedRoles = new List<string> { StaticUserRole.ADMIN, StaticUserRole.ProjectManager };
                if(!HasAnyRole(this._http.HttpContext, allowedRoles))
                {
                    Console.WriteLine("User does not have permission to create project.");
                    return new GeneralResponse(false, "You do not have permission to create a project.");
                }

                Guid defaultProjectStatus = this._dbContext.ProjectStatuses
                    .Where(ps => ps.ProjectStatusName == StaticProjectStatus.NotStarted)
                    .Select(ps => ps.ProjectStatusId)
                    .FirstOrDefault();
                if(defaultProjectStatus == Guid.Empty)
                {
                    Console.WriteLine($"This status {StaticProjectStatus.NotStarted} does not exist.");
                    return new GeneralResponse(false, $"This status {StaticProjectStatus.NotStarted} does not exist.");
                }

                Guid defaultProjectType = this._dbContext.ProjectTypes
                    .Where(pt => pt.ProjectTypeName == StaticProjectType.Agile)
                    .Select(pt => pt.ProjectTypeId)
                    .FirstOrDefault();
                if(defaultProjectType == Guid.Empty)
                {
                    Console.WriteLine($"This type {StaticProjectType.Agile} does not exist.");
                    return new GeneralResponse(false, $"This type {StaticProjectType.Agile} does not exist.");
                }

                //var userProducts = await this._dbContext.Products
                //    .Where(p => p.CreatedBy == userId)
                //    .Select(p => new { p.ProductId, p.ProductName })
                //    .ToListAsync();
                //if (!userProducts.Any())
                //{
                //    Console.WriteLine($"No products found for user {userId}.");
                //    return new GeneralResponse(false, "You must have at least one product to create a project.");
                //}

                //if(productId == Guid.Empty || !userProducts.Any(p => p.ProductId == productId))
                //{
                //    Console.WriteLine($"Invalid or no product selected for user {userId}.");
                //    return new GeneralResponse(false, "Please select a valid product.");
                //}

                PMIS_Project newProject = new PMIS_Project
                {
                    ProjectName = model.ProjectName,
                    Description = model.Description,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    ProjectStatusId = defaultProjectStatus,
                    ProjectTypeId = defaultProjectType,
                    ProductId = productId,
                    UserId = userId,
                };

                this._dbContext.Projects.Add(newProject);
                await this._dbContext.SaveChangesAsync();

                return new GeneralResponse(true, "Created project successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new GeneralResponse(false, ex.Message);
            }
        }

        public async Task<List<ProjectDTO>> GetProjectsAsync(Guid productId)
        {
            try
            {
                if(productId == Guid.Empty)
                {
                    Console.WriteLine("What the fuck did you put into URL?");
                    return new List<ProjectDTO>();
                }

                if (this._http.HttpContext?.User == null)
                {
                    Console.WriteLine("HttpContext or User is null.");
                    return new List<ProjectDTO>();
                }

                string? userId = this._http.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    Console.WriteLine("User ID not found in HttpContext.");
                    return new List<ProjectDTO>();
                }

                List<string> allowedRoles = new List<string> { StaticUserRole.ADMIN, StaticUserRole.ProjectManager };
                if (!HasAnyRole(this._http.HttpContext, allowedRoles))
                {
                    Console.WriteLine("User does not have permission to view projects.");
                    return new List<ProjectDTO>();
                }

                var products = await this._dbContext.Products
                 .Where(p => p.ProductId == productId)
                 .Select(p => new { p.ProductId })
                 .ToListAsync();
                if (!products.Any())
                {
                    Console.WriteLine($"No projects for this product {productId}.");
                    return new List<ProjectDTO>();
                }


                List<PMIS_Project> projects = await this._dbContext.Projects
                    .Where(p => p.ProductId == productId)
                    .ToListAsync();

                if (!projects.Any())
                {
                    Console.WriteLine($"No projects for this product {productId}");
                    return new List<ProjectDTO>();
                }

                List<ProjectDTO> projectDTOs = Mapper.MapProjectToProjectDTOByList(projects);
                return projectDTOs;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving projects: {ex.Message}");
                throw;
            }
        }

        public async Task<ProjectDTO> GetProjectById(Guid projectId)
        {
            try
            {
                if(GeneralService.IsHttpContextOrUserNull(this._http.HttpContext!))
                {
                    Console.WriteLine("HttpContext or User is null.");
                    return new ProjectDTO { Message = "HttpContext or User is null." };
                }

                string? userId = GeneralService.GetUserIdFromContext(this._http.HttpContext!);
                if (string.IsNullOrEmpty(userId))
                {
                    Console.WriteLine("User ID not found in HttpContext.");
                    return new ProjectDTO { Message = "User ID not found in HttpContext." };
                }

                List<string> allowedRoles = new List<string> { StaticUserRole.ADMIN, StaticUserRole.ProjectManager };
                if (!HasAnyRole(this._http.HttpContext!, allowedRoles))
                {
                    Console.WriteLine("User does not have permission to view projects.");
                    return new ProjectDTO { Message = "User does not have permission to view projects." };
                }

                PMIS_Project? project = await this._dbContext.Projects
                    .FirstOrDefaultAsync(p => p.ProjectId == projectId);
                if(project == null)
                {
                    Console.WriteLine("There is not project information. Please check again!");
                    return new ProjectDTO { Message = "There is not project information. Please check again!" };
                }

                ProjectDTO projectDTO = Mapper.MapProjectToProjectDTO(project);
                return projectDTO;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        public async Task<ProjectDTO> EditProjectByIdAsync(Guid projectId, EditProjectRequest model)
        {
            try
            {
                if(GeneralService.IsHttpContextOrUserNull(this._http.HttpContext!))
                {
                    Console.WriteLine("HttpContext or User is null.");
                    return new ProjectDTO { Message = "HttpContext or User is null." };
                }

                string? userId = GeneralService.GetUserIdFromContext(this._http.HttpContext!);
                if (string.IsNullOrEmpty(userId))
                {
                    Console.WriteLine("User ID not found in HttpContext.");
                    return new ProjectDTO { Message = "User ID not found in HttpContext." };
                }

                List<string> allowedRoles = new List<string> { StaticUserRole.ADMIN, StaticUserRole.ProjectManager };
                if (!HasAnyRole(this._http.HttpContext!, allowedRoles))
                {
                    Console.WriteLine("User does not have permission to edit project.");
                    return new ProjectDTO { Message = "User does not have permission to edit project." };
                }

                if (model == null)
                {
                    Console.WriteLine("Edit project model is null.");
                    return new ProjectDTO { Message = "Edit project model is null." };
                }

                PMIS_Project? project = await this._dbContext.Projects
                    .FirstOrDefaultAsync(p => p.ProjectId == projectId);
                if (project == null)
                {
                    Console.WriteLine($"Project with ID {projectId} not found.");
                    return new ProjectDTO { Message = $"Project with ID {projectId} not found." };
                }

                var validStatuses = await this._dbContext.ProjectStatuses
                    .Select(ps => new { ps.ProjectStatusId, ps.ProjectStatusName })
                    .ToListAsync();

                var validTypes = await this._dbContext.ProjectTypes
                    .Select(pt => new { pt.ProjectTypeId, pt.ProjectTypeName })
                    .ToListAsync();

                project.ProjectName = string.IsNullOrEmpty(model.ProjectName) ? project.ProjectName : model.ProjectName;
                project.Description = string.IsNullOrEmpty(model.Description) ? project.Description : model.Description;
                project.StartDate = model.StartDate != null ? model.StartDate : project.StartDate;
                project.EndDate = model.EndDate != null ? model.EndDate : project.EndDate;
                if (model.ProjectStatusId != Guid.Empty)
                {
                    if (!validStatuses.Any(ps => ps.ProjectStatusId == model.ProjectStatusId))
                    {
                        Console.WriteLine($"Invalid ProjectStatusId: {model.ProjectStatusId}");
                        return new ProjectDTO { Message = "Invalid Project Status selected." };
                    }
                    project.ProjectStatusId = model.ProjectStatusId;
                }

                if (model.ProjectTypeId != Guid.Empty)
                {
                    if (!validTypes.Any(pt => pt.ProjectTypeId == model.ProjectTypeId))
                    {
                        var validTypeNames = string.Join(", ", validTypes.Select(pt => pt.ProjectTypeName));
                        Console.WriteLine($"Invalid ProjectTypeId: {model.ProjectTypeId}. Available types: {validTypeNames}");
                        return new ProjectDTO { Message = $"Invalid Project Type selected. Available options: {validTypeNames}" };
                    }
                    project.ProjectTypeId = model.ProjectTypeId;
                }

                await this._dbContext.SaveChangesAsync();

                ProjectDTO projectDTO = Mapper.MapProjectToProjectDTO(project);
                return projectDTO;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        public async Task<ProjectDTO> AssignProjectManagerToProjectAsync(string userId, Guid projectId, string token)
        {
            try
            {
                // Tìm token
                var inviteToken = await _dbContext.Invites
                    .Include(it => it.Project)
                    .FirstOrDefaultAsync(it => it.Token == token && it.ProjectId == projectId);

                if (inviteToken == null)
                {
                    return new ProjectDTO { Message = "Token không hợp lệ." };
                }

                if (inviteToken.IsUsed || inviteToken.ExpiresAt < DateTime.UtcNow)
                {
                    return new ProjectDTO { Message = "Token đã được sử dụng hoặc hết hạn." };
                }

                var project = inviteToken.Project;
                if (project == null)
                {
                    return new ProjectDTO { Message = "Dự án không tồn tại." };
                }

                if (!string.IsNullOrEmpty(project.ProductManager))
                {
                    return new ProjectDTO { Message = "Dự án đã có project manager rồi." };
                }

                var userToInvite = await _dbContext.Users
                    .FirstOrDefaultAsync(u => u.Id == userId);
                if (userToInvite == null)
                {
                    return new ProjectDTO { Message = "Người dùng không tồn tại." };
                }

                var isPMInOtherProject = await _dbContext.Projects
                    .AnyAsync(p => p.ProductManager == userId && p.ProjectId != projectId);
                if (isPMInOtherProject)
                {
                    return new ProjectDTO { Message = "Người này đã là project manager ở dự án khác." };
                }

                // Kiểm tra và thêm role ProjectManager nếu cần
                var projectManagerRole = await _roleManager.FindByNameAsync("ProjectManager");
                if (projectManagerRole == null)
                {
                    projectManagerRole = new IdentityRole("ProjectManager");
                    var roleResult = await _roleManager.CreateAsync(projectManagerRole);
                    if (!roleResult.Succeeded)
                    {
                        return new ProjectDTO { Message = "Không thể tạo role ProjectManager: " + string.Join(", ", roleResult.Errors.Select(e => e.Description)) };
                    }
                }

                // Kiểm tra xem user đã có role ProjectManager chưa
                var hasRole = await _dbContext.UserRoles
                    .AnyAsync(ur => ur.UserId == userId && ur.RoleId == projectManagerRole.Id);
                if (!hasRole)
                {
                    _dbContext.UserRoles.Add(new IdentityUserRole<string>
                    {
                        UserId = userId,
                        RoleId = projectManagerRole.Id
                    });
                }

                // Cập nhật project manager
                project.ProductManager = userId;

                // Cập nhật invite token
                inviteToken.InvitedUserId = userId;
                inviteToken.IsUsed = true;

                // Lưu tất cả thay đổi
                await _dbContext.SaveChangesAsync();

                return new ProjectDTO
                {
                    ProjectId = project.ProjectId,
                    ProjectName = project.ProjectName,
                    Description = project.Description,
                    StartDate = project.StartDate,
                    EndDate = project.EndDate,
                    ProjectStatusId = project.ProjectStatusId,
                    ProductId = project.ProductId,
                    UserId = project.UserId,
                    ProjectTypeId = project.ProjectTypeId,
                    ProductManager = project.ProductManager,
                    Message = "Project manager assigned successfully."
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return new ProjectDTO { Message = $"Lỗi khi gán project manager: {ex.Message}" };
            }
        }

        public async Task<string> GenerateInviteTokenAsync(Guid projectId)
        {
            var project = await _dbContext.Projects
                .FirstOrDefaultAsync(p => p.ProjectId == projectId);

            if (project == null)
            {
                throw new Exception("Project not found.");
            }

            var token = Guid.NewGuid().ToString();
            var inviteToken = new PMIS_Invite
            {
                Token = token,
                ProjectId = projectId,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(2),
                IsUsed = false
            };

            _dbContext.Invites.Add(inviteToken);
            await _dbContext.SaveChangesAsync();

            return token;
        }

        public async Task<ProjectTeamDTO> AssignTeamToProjectAsync(Guid projectId, AssignTeamToProjectRequest model)
        {
            try
            {
                if (this._http == null || this._http.HttpContext == null)
                {
                    this._logger.LogError("HttpContextAccessor or HttpContext is null.");
                    return new ProjectTeamDTO { Message = "HttpContextAccessor or HttpContext is null." };
                }

                if (_http.HttpContext.User == null)
                {
                    _logger.LogError("User is null.");
                    return new ProjectTeamDTO { Message = "User is null." };
                }

                string? userId = GeneralService.GetUserIdFromContext(_http.HttpContext);
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogError("User ID not found in HttpContext.");
                    return new ProjectTeamDTO { Message = "User ID not found in HttpContext." };
                }

                if (!_http.HttpContext.User.IsInRole(StaticUserRole.ProjectManager))
                {
                    _logger.LogWarning("User does not have permission to assign team to project.");
                    return new ProjectTeamDTO { Message = "User does not have permission to assign team to project." };
                }

                //Guid projectId = await this._dbContext.Projects
                //    .Where(p => p.ProjectId == model.ProjectId)
                //    .Select(p => p.ProjectId)
                //    .FirstOrDefaultAsync();
                if (projectId == Guid.Empty)
                {
                    this._logger.LogWarning($"Project ID not found.");
                    return new ProjectTeamDTO { Message = "Project ID not found." };
                }

                Guid teamId = await this._dbContext.Teams
                   .Where(t => t.TeamId == model.TeamId)
                   .Select(t => t.TeamId)
                   .FirstOrDefaultAsync();
                if (teamId == Guid.Empty)
                {
                    this._logger.LogWarning($"Team ID not found.");
                    return new ProjectTeamDTO { Message = "Team ID not found." };
                }

                this._logger.LogInformation("Updating data....");
                PMIS_ProjectTeam projectTeam = new PMIS_ProjectTeam
                {
                    ProjectId = projectId,
                    TeamId = teamId,
                    RoleOfTeam = model.RoleOfTeam,
                    AssignedDate = model.AssignedDate = DateTime.UtcNow,
                    CreatedBy = userId,
                };
                this._dbContext.ProjectTeams.Add(projectTeam);
                await this._dbContext.SaveChangesAsync();

                this._logger.LogInformation("Assigned team to project successfully.");
                ProjectTeamDTO projectTeamDTO = Mapper.MapProjectTeamToProjectTeamDTO(projectTeam);
                return projectTeamDTO;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<GeneralResponse> DeleteProjectAsync(Guid projectId)
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
                List<string> allowedRoles = new List<string> { StaticUserRole.ProjectManager };
                if (!HasAnyRole(this._http.HttpContext!, allowedRoles))
                {
                    Console.WriteLine("User does not have permission to delete this project.");
                    return new GeneralResponse(false, "User does not have permission to delete this project.");
                }

                PMIS_Project? project = await this._dbContext.Projects
                    .Where(p => p.ProjectId == projectId)
                    .FirstOrDefaultAsync();
                if (project == null)
                {
                    Console.WriteLine("Project not found.");
                    return new GeneralResponse(false, "Project not found.");
                }

                this._dbContext.Projects.Remove(project);
                await this._dbContext.SaveChangesAsync();
                return new GeneralResponse(true, "Deleted project successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new GeneralResponse(false, ex.Message);
            }
        }

        public ProjectDTO UpdateProjectStatus(Guid projectId, EditProjectStatusRequest request)
        {
            if (projectId == Guid.Empty)
            {
                this._logger.LogError("Project ID is empty.");
                return new ProjectDTO { Message = "Project ID is empty." };
            }

            try
            {
                // Raw SQL query to update the project status
                string sqlQuery = "UPDATE Projects SET ProjectStatusId = {0} WHERE ProjectId = {1}";

                // Execute the raw SQL query
                int rowsAffected = this._dbContext.Database.ExecuteSqlRaw(sqlQuery, request.ProjectStatusId, projectId);

                if (rowsAffected == 0)
                {
                    this._logger.LogError("Project not found or no rows affected.");
                    return new ProjectDTO { Message = "Project not found or no rows affected." };
                }

                // Optionally, retrieve the updated project to return as DTO
                PMIS_Project? project = this._dbContext.Projects
                    .FirstOrDefault(p => p.ProjectId == projectId);

                if (project == null)
                {
                    this._logger.LogError("Project not found after update.");
                    return new ProjectDTO { Message = "Project not found after update." };
                }

                ProjectDTO projectDTO = Mapper.MapProjectToProjectDTO(project);
                return projectDTO;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex.Message);
                return new ProjectDTO { Message = ex.Message };
            }
        }

        #region extra functions
        private bool HasAnyRole(HttpContext httpContext, List<string> allowedRoles)
        {
            // Lấy danh sách vai trò từ HttpContext.User
            var userRoles = httpContext.User.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

            // Kiểm tra xem user có bất kỳ vai trò nào trong allowedRoles không
            return userRoles.Any(role => allowedRoles.Contains(role, StringComparer.OrdinalIgnoreCase));
        }
        #endregion
    }
}
