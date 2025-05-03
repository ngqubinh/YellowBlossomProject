using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using YellowBlossom.Application.DTOs.General;
using YellowBlossom.Application.DTOs.Project;
using YellowBlossom.Application.Interfaces.PMIS;

namespace YellowBlossom.Presentation.Controllers
{ 

    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _project;

        public ProjectController(IProjectService project)
        {
            this._project = project;
        }

        [HttpGet("products/{productId}/projects")]
        public async Task<ActionResult<List<ProjectDTO>>> GetProjectAsync(Guid productId)
        {
            List<ProjectDTO> res = await this._project.GetProjectsAsync(productId);
            return Ok(res);
        }

        [HttpPost("products/{productId}/create-project")]
        public async Task<ActionResult<GeneralResponse>> CreateProjectAsync(Guid productId, CreateProjectRequest model)
        {
            GeneralResponse res = await this._project.CreateProjectAsync(productId, model);
            if(!ModelState.IsValid || res.Success==false)
            {
                return BadRequest(res);
            }
            return Ok(res);
        }

        [HttpGet("projects/{projectId}/details")]
        public async Task<ActionResult<ProjectDTO>> GetProjectByIdAsync(Guid projectId)
        {
            if (projectId == Guid.Empty)
            {
                return BadRequest("Project ID is required.");
            }

            ProjectDTO? res = await this._project.GetProjectById(projectId);
            if (res == null || res.ProjectId == Guid.Empty)
            {
                return NotFound("Project not found.");
            }

            if (HttpContext.User?.Identity == null || !HttpContext.User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }

            return Ok(res);
        }
        
        [HttpPost("projects/{projectId}/edit")]
        public async Task<ActionResult<ProjectDTO>> EditProjectAsync(Guid projectId, EditProjectRequest model)
        {
            if (projectId == Guid.Empty)
            {
                return BadRequest("Project ID is required.");
            }

            ProjectDTO res = await this._project.EditProjectByIdAsync(projectId, model);
            if(!ModelState.IsValid)
            {
                return BadRequest(res);
            }
            return Ok(res);
        }

        [HttpPost("{projectId}/invite")]
        public async Task<IActionResult> GenerateInviteToken(Guid projectId)
        {
            var token = await this._project.GenerateInviteTokenAsync(projectId);
            return Ok(new { Token = token });
        }

        [HttpPost("{projectId}/accept")]
        public async Task<IActionResult> AcceptInvite(Guid projectId, [FromBody] AcceptInviteRequest request)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { Message = "Không tìm thấy user trong credentials." });
                }

                var projectDto = await _project.AssignProjectManagerToProjectAsync(userId, projectId, request.Token);
                return Ok(projectDto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPost("{projectId}/assign-team-to-project")]
        public async Task<ActionResult<ProjectTeamDTO>> AssignTeamToProjectAsync(Guid projectId, AssignTeamToProjectRequest model)
        {
            if(projectId == Guid.Empty)
            {
                return NotFound(projectId);
            }

            ProjectTeamDTO res = await this._project.AssignTeamToProjectAsync(projectId, model);
            if(!ModelState.IsValid)
            {
                return BadRequest(res);
            }
            return Ok(res);
        }

        [HttpDelete("projects/{projectId}/delete")]
        public async Task<ActionResult<GeneralResponse>> DeleteProjectAsync(Guid projectId)
        {
            GeneralResponse res = await this._project.DeleteProjectAsync(projectId);
            return Ok(res);
        }
        public class AcceptInviteRequest
        {
            public string Token { get; set; } = string.Empty;
        }
    }
}
