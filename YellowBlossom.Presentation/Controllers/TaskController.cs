using Microsoft.AspNetCore.Mvc;
using YellowBlossom.Application.DTOs.General;
using YellowBlossom.Application.DTOs.Task;
using YellowBlossom.Application.Interfaces.PMIS;

namespace YellowBlossom.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _task;

        public TaskController(ITaskService task)
        {
            this._task = task;
        }

        [HttpPost("{projectId}/create-task")]
        public async Task<ActionResult<TaskDTO>> CreateTaskAsync(Guid projectId, CreateTaskRequest model)
        {
            if(projectId == Guid.Empty)
            {
                return NotFound("Project ID not found.");
            }

            TaskDTO res = await _task.CreateTaskAsync(projectId, model);
            if(!ModelState.IsValid)
            {
                return BadRequest(res);
            }
            return Ok(res);
        }

        [HttpGet("{projectId}/tasks")]
        public async Task<ActionResult<List<TaskDTO>>> GetListOfTasksAsync(Guid projectId)
        {
            List<TaskDTO> res = await this._task.GetListOfTasksAsync(projectId);
            return Ok(res);
        }

        [HttpPost("{projectId}/{taskId}/assign")]
        public async Task<ActionResult<TaskDTO>> AssignTeamToTaskAsync(Guid taskId, AssignTeamToTaskRequest model)
        {
            if(taskId == Guid.Empty)
            {
                return NotFound("Task ID not found.");
            }

            TaskDTO res = await this._task.AssignTeamToTask(taskId, model);
            if(!ModelState.IsValid)
            {
                return BadRequest(res);
            }
            return Ok(res);
        }
        
        [HttpGet("{projectId}/{taskId}/details")]
        public async Task<ActionResult<TaskDTO>> GetTaskDetails(Guid taskId)
        {
            TaskDTO res = await this._task.GetTaskDetailsAsync(taskId);
            return Ok(res);   
        }
        
        [HttpPost("{projectId}/{taskId}/update-status")]
        public async Task<ActionResult<TaskDTO>> UpdateTaskStatusAsyn(Guid taskId, UpdateTaskStatusRequest model)
        {
            TaskDTO res = await this._task.UpdateTaskStatusAsync(taskId, model);
            if(!ModelState.IsValid)
            {
                return BadRequest(res);
            }
            return Ok(res);
        }

        [HttpPost("{projectId}/{taskId}/edit")]
        public async Task<ActionResult<TaskDTO>> EditTaskAsync(Guid taskId, EditTaskRequest model)
        {
            if(taskId == Guid.Empty)
            {
                return NotFound("Task Id not found.");
            }
            TaskDTO res = await this._task.EditTaskAsync(taskId, model);
            if(!ModelState.IsValid)
            {
                return BadRequest(res);
            }
            return Ok(res);
        }

        [HttpPost("{projectId}/{taskId}/delete")]
        public async Task<ActionResult<GeneralResponse>> DeleteTaskAsync(Guid taskId)
        {
            GeneralResponse res = await this._task.DeleteTaskAsync(taskId);
            if(res.Success==false)
            {
                return BadRequest(res);
            }
            return Ok(res);
        }

        [HttpPost("send-deadline-reminder")]
        public async Task<ActionResult<GeneralResponse>> SendDeadlineReminderAsync()
        {
            GeneralResponse res = await this._task.SendDeadlineReminderAsync();
            return Ok(res);
        }
    }
}
