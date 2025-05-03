using Microsoft.AspNetCore.Mvc;
using YellowBlossom.Application.DTOs.General;
using YellowBlossom.Application.DTOs.Team;
using YellowBlossom.Application.Interfaces.PMIS;

namespace YellowBlossom.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamController : ControllerBase
    {
        private readonly ITeamService _team;

        public TeamController(ITeamService team)
        {
            this._team = team;
        }

        [HttpPost("create-team")]
        public async Task<ActionResult<TeamDTO>> CreateTeam(CreateTeamRequest model)
        {
            TeamDTO res = await this._team.CreateTeamAsync(model);
            if (res == null)
            {
                return NotFound(res);
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(res);
            }
            return Ok(res);
        }

        [HttpGet("teams")]
        public async Task<ActionResult<List<TeamDTO>>> GetTeams()
        {
            List<TeamDTO> res = await this._team.GetTeamsByProductManager();
            return Ok(res);
        }

        [HttpPost("assign-user-to-team")]
        public async Task<ActionResult<UserTeamDTO>> AssignUserToTeam(AssignTeamRequest model)
        {
            UserTeamDTO res = await this._team.AssignUserToTeamAsync(model);
            if (res == null)
            {
                return NotFound(res);
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(res);
            }
            return Ok(res);
        }

        [HttpGet("current-teams")]
        public async Task<ActionResult<List<TeamDTO>>> GetCurrentTeams()
        {
            List<TeamDTO> res = await this._team.GetCurrentTeamsAsync();
            return Ok(res);
        }

        [HttpGet("users/{userId}/details")]
        public async Task<ActionResult<UserDTO>> GetUserDetailsAsync(string userId)
        {
            UserDTO res = await this._team.GetUserDetailsAsync(userId);
            return Ok(res);
        }

        [HttpPost("teams/update-user-roles")]
        public async Task<ActionResult<GeneralResponse>> UpdateUserRolesAsync([FromBody]UpdateUserRolesRequest model)
        {
            GeneralResponse res = await this._team.UpdateUserRolesAsync(model);
            if(!ModelState.IsValid || model == null)
            {
                return BadRequest(res);
            }
            return Ok(res);
        }

        [HttpPost("users/{userId}/lock")]
        public async Task<ActionResult<GeneralResponse>> LockUserAsync([FromBody]UpdateUserLockRequest request)
        {
            GeneralResponse res = await this._team.UpdateUserLockStatusAsync(request);
            return Ok(res);
        }

        [HttpPost("users/{userId}/unlock")]
        public async Task<ActionResult<GeneralResponse>> UnLockUserAsync([FromBody]UpdateUserLockRequest request)
        {
            GeneralResponse res = await this._team.UpdateUserLockStatusAsync(request);
            return Ok(res);
        }
    }
}
