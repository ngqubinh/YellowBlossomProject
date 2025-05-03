using Microsoft.AspNetCore.Mvc;
using YellowBlossom.Application.DTOs.General;
using YellowBlossom.Application.DTOs.Test;
using YellowBlossom.Application.Interfaces.PMIS;

namespace YellowBlossom.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly ITestService _test;

        public TestController(ITestService test)
        {
            this._test = test;
        }

        [HttpPost("teams/{teamId}/tasks/{taskId}/create-test-case")]
        public async Task<ActionResult<TestCaseDTO>> CreateTestCaseAsync(Guid teamId, Guid taskId, [FromBody] CreateTestCaseRequest request)
        {
            TestCaseDTO res = await this._test.CreateTestCaseAsync(teamId, taskId, request);
            if (!ModelState.IsValid)
            {
                return BadRequest(res);
            }
            return Ok(res);
        }

        [HttpGet("teams/{teamId}/testcases")]
        public async Task<ActionResult<List<TestCaseDTO>>> GetAllTestCasesViaTeamAsync()
        {
            List<TestCaseDTO> res = await this._test.GetAllTestCasesViaTeamAsync();
            return Ok(res);
        }

        [HttpGet("testcases/all")]
        public async Task<ActionResult<List<TestCaseDTO>>> GetAllTestCasesForProjectManagerAsync()
        {
            List<TestCaseDTO> res = await this._test.GetAllTestCasesForProjectManagerAsync();
            return Ok(res);
        }

        [HttpGet("testruns/all")]
        public async Task<ActionResult<List<TestRunDTO>>> GetAllTestRunsForProjectManagerAsync()
        {
            List<TestRunDTO> res = await this._test.GetAllTestRunsForProjectManagerAsync();
            return Ok(res);
        }

        [HttpGet("testcases/{testCaseId}/details")]
        public async Task<ActionResult<TestCaseDTO>> GetTestCaseDetailsAsync(Guid testCaseId)
        {
            TestCaseDTO res = await this._test.GetTestCaseDetailsAsync(testCaseId);
            return Ok(res);
        }

        [HttpPut("testcases/{testCaseId}/edit")]
        public async Task<ActionResult<TestCaseDTO>> EditTestCaseAsync(Guid testCaseId, EditTestCaseRequest request)
        {
            if(testCaseId == Guid.Empty)
            {
                return NotFound("Test Case ID not found.");
            }
            TestCaseDTO res = await this._test.EditTestCasesAsync(testCaseId, request);
            if (res == null || request == null)
            {
                return BadRequest(res);
            }
            return Ok(res);
        }

        [HttpDelete("testcases/{testCaseId}/delete")]
        public async Task<ActionResult<GeneralResponse>> DeleteTestCaseAsync(Guid testCaseId)
        {
            if (testCaseId == Guid.Empty)
            {
                return NotFound("Test Case ID not found.");
            }
            GeneralResponse res = await this._test.DeleteTestCaseAsync(testCaseId);
            if (res.Success == false)
            {
                return BadRequest(res);
            }
            return Ok(res);
        }

        [HttpPost("teams/{teamId}/tasks/{taskId}/create-test-run")]
        public async Task<ActionResult<TestRunDTO>> CreateTestRunAsync(Guid teamId, Guid taskId, [FromBody]CreateTestRunRequest request)
        {
            TestRunDTO res = await this._test.CreateTestRunsAsync(teamId, taskId, request);
            if (!ModelState.IsValid)
            {
                return BadRequest(res);
            }
            return Ok(res);
        }

        [HttpGet("testruns/{testRunId}/details")]
        public async Task<ActionResult<TestRunDTO>> GetTestRunDetailsAsync(Guid testRunId)
        {
            if (testRunId == Guid.Empty)
            {
                return NotFound("Test Run Id not found.");
            }
            TestRunDTO res = await this._test.GetTestRunDetailsAsync(testRunId);
            return Ok(res);
        }

        [HttpPut("testruns/{testRunId}/edit")]
        public async Task<ActionResult<TestRunDTO>> EditTestRunAsync(Guid testRunId, [FromBody]EditTestRunRequest request)
        {
            if (testRunId == Guid.Empty)
            {
                return NotFound("Test Run Id not found.");
            }
            TestRunDTO res = await this._test.EditTestRunAsync(testRunId, request);
            if (res == null || request == null)
            {
                return BadRequest(res);
            }
            return Ok(res);
        }

        [HttpDelete("testruns/{testRunId}/delete")]
        public async Task<ActionResult<GeneralResponse>> DeleteTestRunAsync(Guid testRunId)
        {
            if (testRunId == Guid.Empty)
            {
                return NotFound("Test Run Id not found.");
            }
            GeneralResponse res = await this._test.DeleteTestCaseAsync(testRunId);
            if (res.Success == false)
            {
                return BadRequest(res);
            }
            return Ok(res);
        }

        [HttpPost("test-runs/{testRunId}/test-cases/{testCaseId}/results")]
        public async Task<ActionResult<TestRunTestCaseDTO>> UpdateTestRunTestCaseAsyn(Guid testRunId, Guid testCaseId, [FromBody] UpdateTestRunTestCaseRequest request)
        {
            if (string.IsNullOrEmpty(request.ActualResult))
            {
                return BadRequest(new TestRunTestCaseDTO { Message = "Actual result cannot be empty." });
            }
            TestRunTestCaseDTO res = await this._test.UpdateTestRunTestCaseAsync(testRunId, testCaseId, request);
            if (res == null)
            {
                return StatusCode(500, new { message = "Internal server error while updating test case." });
            }
            return Ok(res);
        }

        [HttpGet("tests/summary")]
        public async Task<ActionResult<TestSummaryDTO>> GetTestSummaryAsync()
        {
            TestSummaryDTO res  = await this._test.GetTestSummaryAsync();
            return Ok(res);
        }

        [HttpGet("test-runs/{testRunId}/test-cases/{testCaseId}/history")]
        public async Task<ActionResult<List<TestRunHistoryDTO>>> GetTestRunHistoryAsync(Guid testRunId, Guid testCaseId)
        {
            List<TestRunHistoryDTO> res = await this._test.GetTestRunHistoryAsync(testRunId, testCaseId);
            return Ok(res);
        }
    }
}

