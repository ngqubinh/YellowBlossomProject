using YellowBlossom.Application.DTOs.General;
using YellowBlossom.Application.DTOs.Task;

namespace YellowBlossom.Application.Interfaces.PMIS
{
    public interface ITaskService
    {
        Task<TaskDTO> CreateTaskAsync(Guid projectId, CreateTaskRequest model);
        Task<List<TaskDTO>> GetListOfTasksAsync(Guid projectId);
        Task<TaskDTO> AssignTeamToTask(Guid taskId, AssignTeamToTaskRequest model);
        Task<TaskDTO> GetTaskDetailsAsync(Guid taskId);
        Task<TaskDTO> UpdateTaskStatusAsync(Guid taskId, UpdateTaskStatusRequest model);
        Task<TaskDTO> EditTaskAsync(Guid taskId, EditTaskRequest model);
        Task<GeneralResponse> DeleteTaskAsync(Guid taskId);
        Task<GeneralResponse> SendDeadlineReminderAsync();
    }
}
