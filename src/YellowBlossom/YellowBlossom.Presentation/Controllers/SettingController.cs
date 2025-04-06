using Microsoft.AspNetCore.Mvc;
using YellowBlossom.Application.DTOs.General;
using YellowBlossom.Application.Interfaces.Setting;

namespace YellowBlossom.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingController : ControllerBase
    {
        private readonly IInitService _init;

        public SettingController(IInitService init)
        {
            this._init = init;
        }

        [HttpPost("seed-project-statuses")]
        public async Task<ActionResult<GeneralResponse>> SeedProjectStatusAsync()
        {
            GeneralResponse result = await this._init.SeedProjectStatusesAsync();
            if(result.Success==false)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPost("seed-project-types")]
        public async Task<ActionResult<GeneralResponse>> SeedProjectTypesAsync()
        {
            GeneralResponse result = await this._init.SeedProjectTypesAsync();
            if (result.Success == false)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPost("seed-roles")]
        public async Task<ActionResult<GeneralResponse>> SeedRolesAsync()
        {
            var result = await this._init.SeedRolesAsync();
            if (result.Success == false) return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("create-admin")]
        public async Task<ActionResult<GeneralResponse>> CreateAdminAsync()
        {
            var result = await this._init.CreateInitAdminAsync();
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }
    }
}
