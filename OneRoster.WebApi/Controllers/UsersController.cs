using System.Threading.Tasks;
using EdFi.OneRoster.WebApi.Helpers;
using EdFi.OneRoster.WebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace EdFi.OneRoster.WebApi.Controllers
{
    [Route("[controller]")]
    //[Route(RouteConventions.Default)]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IOneRosterService _service;

        public UsersController(IOneRosterService service)
        {
            _service = service;
        }

        [HttpGet]        
        public async Task<IActionResult> GetUsers([FromQuery] RequestModel request)
        {
            var response = await _service.GetUsersAsync(request);
            Response.Headers.Add("X-Total-Count", response.TotalCount.ToString());
            return Ok(new { users = response.Response });
        }
    }
}
