using System.Threading.Tasks;
using EdFi.OneRoster.WebApi.Helpers;
using EdFi.OneRoster.WebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace EdFi.OneRoster.WebApi.Controllers
{
    [Route(RouteConventions.Default)]
    [ApiController]
    public class CampusVersionController : ControllerBase
    {
        private readonly IOneRosterService _service;

        public CampusVersionController(IOneRosterService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetCampusVersion([FromQuery] RequestModel request)
        {
            var response = await _service.GetCampusVersionAsync(request);
            Response.Headers.Add("X-Total-Count", response.TotalCount.ToString());
            return Ok(response.Response);
        }

    }
}
