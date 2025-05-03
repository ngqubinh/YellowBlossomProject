using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using YellowBlossom.Application.DTOs.General;
using YellowBlossom.Application.DTOs.Team;
using YellowBlossom.Application.Interfaces.PMIS;
using YellowBlossom.Domain.Constants;
using YellowBlossom.Domain.Models.Auth;
using YellowBlossom.Domain.Models.PMIS;
using YellowBlossom.Infrastructure.Data;
using YellowBlossom.Infrastructure.Services;

namespace YellowBlossom.Infrastructure.Repositories.PMIS
{
    public class TeamService : ITeamService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _http;
        private readonly ILogger<TeamService> _logger;
        private readonly UserManager<User> _userManager;

        public TeamService(ApplicationDbContext dbContext, IHttpContextAccessor http, ILogger<TeamService> logger, UserManager<User> userManager)
        {
            this._dbContext = dbContext;
            this._http = http;
            this._logger = logger;
            LogContext.PushProperty("ServiceName", "TeamService");
            this._userManager = userManager;
        }

        public async Task<UserTeamDTO> AssignUserToTeamAsync(AssignTeamRequest model)
        {
            try
            {
                if (GeneralService.IsHttpContextOrUserNull(this._http.HttpContext!))
                {
                    this._logger.LogWarning("HttpContext or User is null.");
                    return new UserTeamDTO { Message = "HttpContext or User is null." };
                }

                if (!this._http.HttpContext!.User.IsInRole(StaticUserRole.ADMIN))
                {
                    this._logger.LogWarning("User does not have permission to assign user into team.");
                    return new UserTeamDTO { Message = "You do not have permission to assign user into a team." };
                }

                Guid teamId = await this._dbContext.Teams
                    .Where(t => t.TeamId == model.TeamId)
                    .Select(t => t.TeamId)
                    .FirstOrDefaultAsync();
                if (teamId == Guid.Empty)
                {
                    this._logger.LogWarning($"Team ID not found.");
                    return new UserTeamDTO { Message = "Team ID not found." };
                }

                PMIS_UserTeam userTeam = new PMIS_UserTeam
                {
                    UserId = model.UserId,
                    TeamId = teamId,
                    AssignedDate = DateTime.UtcNow,
                };
                this._dbContext.UserTeams.Add(userTeam);
                await this._dbContext.SaveChangesAsync();

                UserTeamDTO userTeamDTO = Mapper.MapUserTeamToUserTeamDTO(userTeam);
                return userTeamDTO;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<TeamDTO> CreateTeamAsync(CreateTeamRequest model)
        {
            try
            {
                if (GeneralService.IsHttpContextOrUserNull(this._http.HttpContext!))
                {
                    this._logger.LogWarning("HttpContext or User is null.");
                    return new TeamDTO { Message = "HttpContext or User is null." };
                }

                string? userId = GeneralService.GetUserIdFromContext(this._http.HttpContext!);
                if (string.IsNullOrEmpty(userId))
                {
                    this._logger.LogWarning("User ID not found in HttpContext.");
                    return new TeamDTO { Message = "User ID not found in HttpContext." };
                }

                if (!this._http.HttpContext!.User.IsInRole(StaticUserRole.ADMIN))
                {
                    this._logger.LogWarning("User does not have permission to create team.");
                    return new TeamDTO { Message = "You do not have permission to create a team." };
                }

                bool isValidTeamName = await this._dbContext.Teams
                    .AnyAsync(t => t.TeamName == model.TeamName);
                if (isValidTeamName)
                {
                    this._logger.LogWarning($"There is another team called {model.TeamName}.");
                    return new TeamDTO($"There is another team called {model.TeamName}.");
                }

                this._logger.LogInformation("Saving data into database.");
                PMIS_Team newTeam = new PMIS_Team(model.TeamName, model.Description, userId);
                this._dbContext.Teams.Add(newTeam);
                await this._dbContext.SaveChangesAsync();

                this._logger.LogInformation($"Created {model.TeamName} successfully.");
                TeamDTO teamDTO = Mapper.MapTeamToTeamDTO(newTeam);
                return teamDTO;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex.Message);
                return new TeamDTO { Message = ex.Message };
            }
        }

        public async Task<List<TeamDTO>> GetTeamsByProductManager()
        {
            try
            {
                if (GeneralService.IsHttpContextOrUserNull(this._http.HttpContext!))
                {
                    this._logger.LogWarning("HttpContext or User is null.");
                    return new List<TeamDTO>();
                }

                if (!this._http.HttpContext!.User.IsInRole(StaticUserRole.ADMIN))
                {
                    this._logger.LogWarning("User does not have permission to create team.");
                    return new List<TeamDTO>();
                }

                List<PMIS_Team> teams = await this._dbContext.Teams.ToListAsync();
                if (teams == null)
                {
                    this._logger.LogWarning("Cannot retrieve Team data.");
                    return new List<TeamDTO>();
                }

                List<TeamDTO> teamDTOs = Mapper.MapTeamToTeamDTOByList(teams);
                return teamDTOs;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex.Message);
                throw;
            }
        }
    
        public async Task<List<TeamDTO>> GetCurrentTeamsAsync()
        {
            try
            {
                if (GeneralService.IsHttpContextOrUserNull(this._http.HttpContext!))
                {
                    this._logger.LogWarning("HttpContext or User is null.");
                    return new List<TeamDTO>();
                }

                string? userId = GeneralService.GetUserIdFromContext(this._http.HttpContext!);
                if (string.IsNullOrEmpty(userId))
                {
                    this._logger.LogWarning("User ID not found.");
                    return new List<TeamDTO>();
                }

                List<PMIS_Team> teams = await this._dbContext.UserTeams
                    .Where(ut => ut.UserId == userId)
                    .Select(ut => ut.Team)
                    .ToListAsync();

                if(teams == null || teams.Count == 0)
                {
                    this._logger.LogWarning("No teams found for the current user.");
                    return new List<TeamDTO>();
                }

                List<TeamDTO> teamDTOs = Mapper.MapTeamToTeamDTOByList(teams);
                return teamDTOs;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<UserDTO> GetUserDetailsAsync(string userId)
        {
            try
            {
                if (GeneralService.IsHttpContextOrUserNull(this._http.HttpContext!))
                {
                    Console.WriteLine("HttpContext or User is null.");
                    return new UserDTO { Message = "HttpContext or User is null." };
                }

                if (!this._http.HttpContext!.User.IsInRole(StaticUserRole.ADMIN))
                {
                    Console.WriteLine("User does not have permission to get data.");
                    return new UserDTO { Message = "User does not have permission to get data." };
                }

                User? user = await this._userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    Console.WriteLine("User not found.");
                    return new UserDTO { Message = "User not found." };
                }

                UserDTO userDTO = Mapper.MapUserToUserDTO(user);
                return userDTO;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new UserDTO { Message = ex.Message };
            }
        }

        public async Task<GeneralResponse> UpdateUserRolesAsync(UpdateUserRolesRequest request)
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

                if (!this._http.HttpContext!.User.IsInRole(StaticUserRole.ADMIN))
                {
                    Console.WriteLine("User does not have permission to adjust roles.");
                    return new GeneralResponse(false, "You do not have permission to adjust roles.");
                }

                User? targetUser = await this._userManager.FindByIdAsync(request.UserId);
                if (targetUser == null)
                {
                    Console.WriteLine($"User {request.UserId} not found.");
                    return new GeneralResponse(false, "User not found.");
                }

                var currentUserRoles = await this._userManager.GetRolesAsync(targetUser);
                IdentityResult? removeRolesResult = await this._userManager.RemoveFromRolesAsync(targetUser, currentUserRoles);
                if (!removeRolesResult.Succeeded)
                {
                    return new GeneralResponse(false, "Failed to remove existing roles.");
                }

                var addRolesResult = await this._userManager.AddToRolesAsync(targetUser, request.Roles);
                if (!addRolesResult.Succeeded)
                {
                    return new GeneralResponse(false, "Failed to assign new roles.");
                }
                return new GeneralResponse(true, "User roles updated successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating user roles: {ex.Message}");
                return new GeneralResponse(false, "Internal server error.");
            }
        }

        public async Task<GeneralResponse> UpdateUserLockStatusAsync(UpdateUserLockRequest request)
        {
            try
            {
                // Kiểm tra HttpContext
                if (GeneralService.IsHttpContextOrUserNull(this._http.HttpContext!))
                {
                    Console.WriteLine("HttpContext or User is null.");
                    return new GeneralResponse(false, "HttpContext or User is null.");
                }

                // Lấy `UserId` của người thực hiện
                string? executorUserId = GeneralService.GetUserIdFromContext(this._http.HttpContext!);
                if (string.IsNullOrEmpty(executorUserId))
                {
                    Console.WriteLine("Executor User ID not found in HttpContext.");
                    return new GeneralResponse(false, "Executor User ID not found in HttpContext.");
                }

                // Kiểm tra quyền hạn của người thực hiện (Chỉ `Admin` mới có quyền)
                if (!this._http.HttpContext!.User.IsInRole(StaticUserRole.ADMIN))
                {
                    Console.WriteLine("User does not have permission to manage lock status.");
                    return new GeneralResponse(false, "You do not have permission to manage lock status.");
                }

                // Lấy thông tin `UserId` cần khóa/mở khóa
                var targetUser = await _userManager.FindByIdAsync(request.UserId);
                if (targetUser == null)
                {
                    Console.WriteLine($"User {request.UserId} not found.");
                    return new GeneralResponse(false, "User not found.");
                }

                // Xác định hành động: Khóa hoặc mở khóa
                if (request.Lock)
                {
                    var lockoutEnd = DateTime.UtcNow.AddDays(request.LockDurationDays);
                    var result = await _userManager.SetLockoutEndDateAsync(targetUser, lockoutEnd);
                    if (!result.Succeeded)
                    {
                        return new GeneralResponse(false, "Failed to lock user.");
                    }

                    return new GeneralResponse(true, $"User locked until {lockoutEnd}.");
                }
                else
                {
                    var result = await _userManager.SetLockoutEndDateAsync(targetUser, null);
                    if (!result.Succeeded)
                    {
                        return new GeneralResponse(false, "Failed to unlock user.");
                    }

                    return new GeneralResponse(true, "User unlocked successfully.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating user lock status: {ex.Message}");
                return new GeneralResponse(false, "Internal server error.");
            }
        }
    }
}
