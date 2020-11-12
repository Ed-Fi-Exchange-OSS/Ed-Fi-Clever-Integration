using System.Threading.Tasks;
using EdFi.OneRoster.WebApi.Helpers;
using EdFi.OneRoster.WebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace EdFi.OneRoster.WebApi.Controllers
{
    [Route(RouteConventions.Default)]
    [ApiController]
    public class DemographicsController : ControllerBase
    {
        private readonly IOneRosterService _service;

        public DemographicsController(IOneRosterService service)
        {
            _service = service;
        }

        [HttpGet]        
        public async Task<IActionResult> GetDemographics([FromQuery] RequestModel request)
        {
            var response = await _service.GetDemographicsAsync(request);
            Response.Headers.Add("X-Total-Count", response.TotalCount.ToString());
            return Ok(new { demographics = response.Response });
        }
    }
}
