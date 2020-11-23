using System.Threading.Tasks;
using EdFi.OneRoster.WebApi.Helpers;
using EdFi.OneRoster.WebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace EdFi.OneRoster.WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AcademicSessionsController : ControllerBase
    {
        private readonly IOneRosterService _service;

        public AcademicSessionsController(IOneRosterService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAcademicSessions([FromQuery] RequestModel request)
        {
            var response = await _service.GetAcademicSessionsAsync(request);
            Response.Headers.Add("X-Total-Count", response.TotalCount.ToString());
            return Ok(new { academicsessions = response.Response });
        }
    }
}
