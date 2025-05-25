using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using YellowBlossom.Application.DTOs.General;
using YellowBlossom.Application.DTOs.Task;
using YellowBlossom.Application.Interfaces.PMIS;
using YellowBlossom.Domain.Constants;
using YellowBlossom.Domain.Models.PMIS;
using YellowBlossom.Infrastructure.Data;
using YellowBlossom.Infrastructure.Services;

namespace YellowBlossom.Infrastructure.Repositories.PMIS
{
    public class TaskService : ITaskService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _http;
        private readonly ILogger<TaskService> _logger;

        public TaskService(ApplicationDbContext dbContext, IHttpContextAccessor http, ILogger<TaskService> logger)
        {
            this._dbContext = dbContext;
            this._http = http;
            this._logger = logger;
            LogContext.PushProperty("ServiceName", "TaskService");
        }

        public async Task<TaskDTO> CreateTaskAsync(Guid projectId, CreateTaskRequest model)
        {
            try
            {
                if(GeneralService.IsHttpContextOrUserNull(this._http.HttpContext!))
                {
                    Console.WriteLine("HttpContext or User is null.");
                    return new TaskDTO { Message = "HttpContext or User is null." };
                }

                string? userId = GeneralService.GetUserIdFromContext(this._http.HttpContext!);
                if (string.IsNullOrEmpty(userId))
                {
                    Console.WriteLine("User ID not found in HttpContext.");
                    return new TaskDTO { Message = "User ID not found in HttpContext." };
                }

                if (!this._http.HttpContext!.User.IsInRole(StaticUserRole.ProjectManager))
                {
                    Console.WriteLine("User does not have permission to create task.");
                    return new TaskDTO { Message = "You do not have permission to create a task." };
                }

                Guid defaultPriority = this._dbContext.Priorities
                    .Where(p => p.PriorityName == StaticPriority.Medium)
                    .Select(p => p.PriorityId)
                    .FirstOrDefault();
                if(defaultPriority == Guid.Empty)
                {
                    Console.WriteLine($"This priority {StaticPriority.Medium} does not exist.");
                    return new TaskDTO { Message = $"This priority {StaticPriority.Medium} does not exist." };
                }

                Guid defaultTaskStatus = this._dbContext.TaskStatuses
                    .Where(ts => ts.TaskStatusName == StaticTaskStatus.Open)
                    .Select(ts => ts.TaskStatusId)
                    .FirstOrDefault();
                if(defaultTaskStatus == Guid.Empty)
                {
                    Console.WriteLine($"This status {StaticTaskStatus.Open} does not exist.");
                    return new TaskDTO { Message = $"This status {StaticTaskStatus.Open} does not exist." };
                }

                PMIS_Task newTask = new PMIS_Task
                {
                    Title = model.Title,
                    Description = model.Description,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    PriorityId = defaultPriority,
                    TaskStatusId = defaultTaskStatus,
                    ProjectId = projectId,
                    CreatedBy = userId,
                    AssignedTeam = model.AssignedTo,
                };
                this._dbContext.Tasks.Add(newTask);
                await this._dbContext.SaveChangesAsync();

                TaskDTO taskDTO = Mapper.MapTaskToTaskDTO(newTask);
                return taskDTO;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        public async Task<List<TaskDTO>> GetListOfTasksAsync(Guid projectId)
        {
            try
            {
                if (_http == null || _http.HttpContext == null)
                {
                    _logger.LogError("HttpContextAccessor or HttpContext is null.");
                    return new List<TaskDTO>();
                }

                if (_http.HttpContext.User == null)
                {
                    _logger.LogError("User is null.");
                    return new List<TaskDTO>();
                }

                string? userId = GeneralService.GetUserIdFromContext(_http.HttpContext);
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogError("User ID not found in HttpContext.");
                    return new List<TaskDTO>();
                }

                if (!_http.HttpContext.User.IsInRole(StaticUserRole.ProjectManager))
                {
                    _logger.LogWarning("User does not have permission to get data.");
                    return new List<TaskDTO>();
                }

                _logger.LogInformation("Retrieving task data from database.");
                List<PMIS_Task> tasks = await _dbContext.Tasks
                    .Where(t => t.ProjectId == projectId)
                    .Include(t => t.TaskStatus)
                    .Include(t => t.Priority)
                    .ToListAsync();

                _logger.LogInformation("Mapping task to DTO.");
                List<TaskDTO> taskDTOs = Mapper.MapTaskToTaskDTOByList(tasks);

                if (taskDTOs.Count == 0)
                {
                    _logger.LogInformation("There are no tasks.");
                }

                return taskDTOs;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting list of tasks");
                throw;
            }
        }

        public async Task<TaskDTO> AssignTeamToTask(Guid taskId, AssignTeamToTaskRequest model)
        {
            try
            {
                if (this._http == null || this._http.HttpContext == null)
                {
                    this._logger.LogError("HttpContextAccessor or HttpContext is null.");
                    return new TaskDTO { Message = "HttpContextAccessor or HttpContext is null." };
                }

                if (_http.HttpContext.User == null)
                {
                    _logger.LogError("User is null.");
                    return new TaskDTO { Message = "User is null." };
                }

                string? userId = GeneralService.GetUserIdFromContext(_http.HttpContext);
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogError("User ID not found in HttpContext.");
                    return new TaskDTO { Message = "User ID not found in HttpContext." };
                }

                if (!_http.HttpContext.User.IsInRole(StaticUserRole.ProjectManager))
                {
                    _logger.LogWarning("User does not have permission to assign team to task.");
                    return new TaskDTO { Message = "User does not have permission to assign team to task." };
                }

                var task = await this._dbContext.Tasks.FindAsync(taskId);
                if (task == null)
                {
                    _logger.LogError("Task not found.");
                    return new TaskDTO { Message = "Task not found." };
                }

                var isTeamInProject = await _dbContext.ProjectTeams
                    .AnyAsync(pt => pt.TeamId == model.TeamId && pt.ProjectId == task.ProjectId);
                if (!isTeamInProject)
                {
                    _logger.LogWarning("Team is not associated with the project.");
                    return new TaskDTO { Message = "Team is not associated with the project." };
                }

                if (task.AssignedTeam.HasValue && task.AssignedTeam.Value == model.TeamId)
                {
                    this._logger.LogInformation("This team is already assigned to the task.");
                    return new TaskDTO { Message = "This team is already assigned to the task." };
                }

                task.AssignedTeam = model.TeamId;
                await _dbContext.SaveChangesAsync();

                this._logger.LogInformation("Assigned team to task successfully.");
                TaskDTO taskDTO = Mapper.MapTaskToTaskDTO(task);
                return taskDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating task assignment: {ex.Message}");
                return new TaskDTO { Message = "An error occurred while assigning the team to the task." };
            }
        }

        public async Task<TaskDTO> GetTaskDetailsAsync(Guid taskId)
        {
            try
            {
                if (this._http == null || this._http.HttpContext == null)
                {
                    this._logger.LogError("HttpContextAccessor or HttpContext is null.");
                    return new TaskDTO { Message = "HttpContextAccessor or HttpContext is null." };
                }

                if (_http.HttpContext.User == null)
                {
                    _logger.LogError("User is null.");
                    return new TaskDTO { Message = "User is null." };
                }

                string? userId = GeneralService.GetUserIdFromContext(_http.HttpContext);
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogError("User ID not found in HttpContext.");
                    return new TaskDTO { Message = "User ID not found in HttpContext." };
                }

                this._logger.LogInformation("Retriving data...");
                var task = await this._dbContext.Tasks
                    .Where(t => t.TaskId == taskId)
                    .Include(p => p.Priority)
                    .Include(p => p.User)
                    .Include(p => p.TaskStatus)
                    .Include(p => p.Team)
                    .FirstOrDefaultAsync();
                if (task == null)
                {
                    this._logger.LogError("Task ID not found.");
                    return new TaskDTO { Message = "Task not found." };
                }
                this._logger.LogInformation("Retrived Task details successfully.");
                TaskDTO taskDTO = Mapper.MapTaskToTaskDTO(task);
                return taskDTO;
            }
            catch (Exception ex)
            {
                this._logger.LogError($"{ex.Message}", ex);
                throw;
            }
        }

        public async Task<TaskDTO> UpdateTaskStatusAsync(Guid taskId, UpdateTaskStatusRequest model)
        {
            try
            {
                if (this._http == null || this._http.HttpContext == null)
                {
                    this._logger.LogError("HttpContextAccessor or HttpContext is null.");
                    return new TaskDTO { Message = "HttpContextAccessor or HttpContext is null." };
                }

                if (_http.HttpContext.User == null)
                {
                    _logger.LogError("User is null.");
                    return new TaskDTO { Message = "User is null." };
                }

                string? userId = GeneralService.GetUserIdFromContext(_http.HttpContext);
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogError("User ID not found in HttpContext.");
                    return new TaskDTO { Message = "User ID not found in HttpContext." };
                }

                //if (!_http.HttpContext.User.IsInRole(StaticUserRole.Developer))
                //{
                //    _logger.LogWarning("User does not have permission to update task status.");
                //    return new TaskDTO { Message = "User does not have permission to update task status." };
                //}
                var task = await _dbContext.Tasks
                    .Include(t => t.TaskStatus) 
                    .Where(t => t.TaskId == taskId)
                    .FirstOrDefaultAsync();
                if (task == null)
                {
                    _logger.LogError("Task not found.");
                    return new TaskDTO { Message = "Task not found." };
                }
                if (task.TaskStatusId == model.TaskStatusId)
                {
                    this._logger.LogInformation("This status is already assigned.");
                    return new TaskDTO { Message = "This status is already assigned." };
                }



                var taskStatus = await this._dbContext.TaskStatuses
                    .Where(ts => ts.TaskStatusId == model.TaskStatusId)
                    .SingleOrDefaultAsync();
                if (taskStatus == null)
                {
                    _logger.LogError("Task status not found.");
                    return new TaskDTO { Message = "Task status not found." };
                }


                task.TaskStatusId = taskStatus.TaskStatusId;
                await this._dbContext.SaveChangesAsync();

                this._logger.LogInformation("Updated task status successfully.");
                TaskDTO taskDTO = Mapper.MapTaskToTaskDTO(task);
                return taskDTO;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<TaskDTO> EditTaskAsync(Guid taskId, EditTaskRequest model)
        {
            try
            {
                if (this._http == null || this._http.HttpContext == null)
                {
                    this._logger.LogError("HttpContextAccessor or HttpContext is null.");
                    return new TaskDTO { Message = "HttpContextAccessor or HttpContext is null." };
                }

                if (_http.HttpContext.User == null)
                {
                    _logger.LogError("User is null.");
                    return new TaskDTO { Message = "User is null." };
                }

                string? userId = GeneralService.GetUserIdFromContext(_http.HttpContext);
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogError("User ID not found in HttpContext.");
                    return new TaskDTO { Message = "User ID not found in HttpContext." };
                }

                if (!_http.HttpContext.User.IsInRole(StaticUserRole.ProjectManager))
                {
                    _logger.LogWarning("User does not have permission to update task.");
                    return new TaskDTO { Message = "User does not have permission to update task." };
                }

                PMIS_Task? task = await this._dbContext.Tasks
                    .Include(t => t.Priority)
                    .Include(t => t.User)
                    .Include(t => t.Project)
                    .Include(t => t.Team)
                    .Include(t => t.TaskStatus)
                    .FirstOrDefaultAsync(t => t.TaskId == taskId);
                if(task == null)
                {
                    this._logger.LogError($"Task with ID {taskId} not found.");
                    return new TaskDTO { Message = $"Task with ID {taskId} not found." };
                }

                var validPriotities = await this._dbContext.Priorities
                    .Select(p => new { p.PriorityId, p.PriorityName })
                    .ToListAsync();

                task.Title = string.IsNullOrEmpty(model.Title) ? task.Title : model.Title;
                task.Description = string.IsNullOrEmpty(model.Description) ? task.Description : model.Description;
                task.StartDate = model.StartDate ?? task.StartDate;
                task.EndDate = model.EndDate ?? task.EndDate;
                if (model.PriorityId != Guid.Empty)
                {
                    if(!validPriotities.Any(p => p.PriorityId == model.PriorityId))
                    {
                        Console.WriteLine($"Invalid PriorityId: {model.PriorityId}");
                        return new TaskDTO { Message = "Invalid Priority Status selected." };
                    }
                    task.PriorityId = model.PriorityId;
                }

                this._logger.LogInformation("Update task information...");
                await this._dbContext.SaveChangesAsync();

                this._logger.LogInformation("Edited task successfully.");
                return Mapper.MapTaskToTaskDTO(task);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<GeneralResponse> DeleteTaskAsync(Guid taskId)
        {
            try
            {
                // Kiểm tra nếu HttpContext hợp lệ
                if (this._http == null || this._http.HttpContext == null)
                {
                    this._logger.LogError("HttpContextAccessor or HttpContext is null.");
                    return new GeneralResponse(false, "HttpContextAccessor or HttpContext is null.");
                }

                if (_http.HttpContext.User == null)
                {
                    _logger.LogError("User is null.");
                    return new GeneralResponse(false, "User is null.");
                }

                string? userId = GeneralService.GetUserIdFromContext(_http.HttpContext);
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogError("User ID not found in HttpContext.");
                    return new GeneralResponse(false, "User ID not found in HttpContext.");
                }

                if (!_http.HttpContext.User.IsInRole(StaticUserRole.ProjectManager))
                {
                    _logger.LogWarning("User does not have permission to delete tasks.");
                    return new GeneralResponse(false, "You do not have permission to delete tasks.");
                }

                // Tìm kiếm task trong database
                var task = await this._dbContext.Tasks.FindAsync(taskId);
                if (task == null)
                {
                    _logger.LogError($"Task with ID {taskId} not found.");
                    return new GeneralResponse(false, $"Task with ID {taskId} not found.");
                }

                // Xóa task
                _dbContext.Tasks.Remove(task);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation($"Task with ID {taskId} deleted successfully.");
                return new GeneralResponse(true, "Task deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting task: {ex.Message}");
                return new GeneralResponse (false, "An error occurred while deleting the task.");
            }
        }

        public async Task<GeneralResponse> SendDeadlineReminderAsync()
        {
            DateTime today = DateTime.UtcNow.Date;
            List<PMIS_Task> tasks = await this._dbContext.Tasks
                .Where(t => t.EndDate != DateTime.MinValue &&
                    (t.EndDate.Date == today.AddDays(5) ||
                     t.EndDate.Date == today.AddDays(3) ||
                     t.EndDate.Date == today.AddDays(1)))
                .Include(t => t.Team)
                .ToListAsync();

            if (!tasks.Any())
            {
                this._logger.LogError("No tasks due for reminders.");
                return new GeneralResponse(false, "No tasks due for reminders.");
            }

            await Parallel.ForEachAsync(tasks, async (task, cancellationToken) =>
            {
                string message = $"Reminder: Task '{task.Title}' is due in {(task.EndDate.Date - today).Days} days.";

                // Fetch emails via UserTeams (weak entity)
                var teamEmails = await _dbContext.UserTeams
                    .Where(ut => ut.TeamId == task.Team.TeamId)
                    .Select(ut => ut.User.Email)
                    .ToListAsync();

                foreach (var email in teamEmails)
                {
                    await MailService.SendEmailAsync(email, "Task Deadline Reminder", message);
                }
            });
            return new GeneralResponse(true, "Send deadline email successfully.");
        }

        public async Task<List<PriorityDTO>> GetPriorityListAsync()
        {
            List<PMIS_Priority> priorities = await this._dbContext.Priorities.ToListAsync();
            List<PriorityDTO> priorityDTOs = Mapper.MapPriorityToPriorityByList(priorities);
            return priorityDTOs;
        }
    
        public async Task<List<TaskDTO>> GetTasksForTeam()
        {
            try
            {
                if (this._http == null || this._http.HttpContext == null)
                {
                    this._logger.LogError("HttpContextAccessor or HttpContext is null.");
                    return new List<TaskDTO>();
                }

                if (_http.HttpContext.User == null)
                {
                    _logger.LogError("User is null.");
                    return new List<TaskDTO>();
                }

                string? userId = GeneralService.GetUserIdFromContext(_http.HttpContext);
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogError("User ID not found in HttpContext.");
                    return new List<TaskDTO>();
                }

                List<PMIS_Task> tasks = await _dbContext.Tasks
                    .Include(t => t.Team)
                    .Where(t => t.Team.ProjectTeams
                        .Any(pt => pt.Team.UserTeams
                            .Any(ut => ut.User.Id == userId)
                        )
                    )
                    .ToListAsync();


                List<TaskDTO> taskDTOs = Mapper.MapTaskToTaskDTOByList(tasks);
                return taskDTOs;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex.Message);
                return new List<TaskDTO>();
            }
        }

        public async Task<List<TaskStatusDTO>> GetAllTaskStatusesAsync()
        {
            List<PMIS_TaskStatus> taskStatuses = await this._dbContext.TaskStatuses.ToListAsync();
            List<TaskStatusDTO> taskStatusDTOs = Mapper.MapTaskStatusToTaskStatusDTOByList(taskStatuses);
            return taskStatusDTOs;
        }
    }
}