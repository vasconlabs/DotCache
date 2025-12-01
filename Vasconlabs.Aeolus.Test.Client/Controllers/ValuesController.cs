using Microsoft.AspNetCore.Mvc;
using System.Text;
using Vasconlabs.Aeolus.Client.Interfaces;

namespace Vasconlabs.Aeolus.Test.Client.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController(IAeolusCacheService _cache) : ControllerBase
    {
        [HttpPost("set")]
        public async Task<IActionResult> Set()
        {
            await _cache.SetAsync("test-key", Encoding.ASCII.GetBytes("test-key"), TimeSpan.FromSeconds(60));

            return Ok();
        }

        [HttpGet("get")]
        public async Task<IActionResult> Get()
        {
            var data = await _cache.GetAsync("test-key");

            return Ok(Encoding.ASCII.GetString(data.ToArray()));
        }
    }
}
