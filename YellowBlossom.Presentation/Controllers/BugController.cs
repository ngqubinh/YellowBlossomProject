using Microsoft.AspNetCore.Mvc;
using YellowBlossom.Application.DTOs.Bug;
using YellowBlossom.Application.DTOs.General;
using YellowBlossom.Application.Interfaces.PMIS;

namespace YellowBlossom.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BugController : ControllerBase
    {
        private readonly IBugService _bug;

        public BugController(IBugService bug)
        {
            this._bug = bug;
        }

        [HttpGet("bugs")]
        public async Task<ActionResult<List<BugDTO>>> GetListOfBugsAsync()
        {
            List<BugDTO> res = await _bug.GetListOfBugsAsync();
            return Ok(res);
        }

        [HttpPost("bugs/{bugId}/resolve")]
        public async Task<ActionResult<BugDTO>> UpdateBugAsync(Guid bugId, [FromBody]UpdateBugRequest request)
        {
            BugDTO res = await this._bug.UpdateBugResolutionAsync(bugId, request);
            if(res == null)
            {
                return BadRequest(res);
            }
            return Ok(res);
        }

        [HttpGet("bugs/team")]
        public async Task<ActionResult<List<BugDTO>>> GetSolvedBugsByTeamAsync()
        {
            List<BugDTO> res = await this._bug.GetBugsByTeamAsync();
            return Ok(res);
        }

        [HttpDelete("bugs/{bugId}/delete")]
        public async Task<ActionResult<GeneralResponse>> DeleteBugWithoutTestCaseAsync(Guid bugId)
        {
            if(bugId == Guid.Empty)
            {
                return NotFound("There is Bug ID not found.");
            }
            GeneralResponse res = await this._bug.DeleteBugWithoutTestCaseAsync(bugId);
            if(res.Success == false)
            {
                return BadRequest(res);
            }
            return Ok(res);
        }

        [HttpGet("statistics")]
        public async Task<ActionResult<BugStatisticsDTO>> GetBugStatiticsAsync()
        {
            BugStatisticsDTO res = await this._bug.GetBugStatisticsAsync();
            return Ok(res);
        }

        [HttpGet("bugs/{bugId}")]
        public async Task<ActionResult<BugDTO>> GetBugDetailsAsync(Guid bugId)
        {
            if(bugId == Guid.Empty)
            {
                return NotFound("Bug ID not found.");
            }
            BugDTO res = await this._bug.GetBugDetailsAsync(bugId);
            return Ok(res);
        }

        [HttpGet("bugs/testRun/{testRunId}")]
        public async Task<ActionResult<List<BugDTO>>> GetBugsByTestRunAsync(Guid testRunId)
        {
            if (testRunId == Guid.Empty)
            {
                return NotFound("Test Run ID not found.");
            }
            List<BugDTO> res = await this._bug.GetBugsByTestRunAsync(testRunId);
            return Ok(res);
        }

        [HttpPut("bugs/{bugId}/edit")]
        public async Task<ActionResult<BugDTO>> EditBugAsync(Guid bugId, [FromBody]EditBugRequest request)
        {
            if (bugId == Guid.Empty)
            {
                return NotFound("Bug ID not found.");
            }
            BugDTO res = await this._bug.EditBugAsync(bugId, request);
            if (res == null || request == null)
            {
                return BadRequest(res);
            }
            return Ok(res);
        }
    }
}
