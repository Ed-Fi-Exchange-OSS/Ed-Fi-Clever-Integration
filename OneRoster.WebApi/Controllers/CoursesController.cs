using System.Threading.Tasks;
using EdFi.OneRoster.WebApi.Helpers;
using EdFi.OneRoster.WebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace EdFi.OneRoster.WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly IOneRosterService _service;

        public CoursesController(IOneRosterService service)
        {
            _service = service;
        }

        [HttpGet]        
        public async Task<IActionResult> GetCourses([FromQuery] RequestModel request)
        {
            var response = await _service.GetCoursesAsync(request);
            Response.Headers.Add("X-Total-Count", response.TotalCount.ToString());
            return Ok(new { courses = response.Response });
        }
    }
}
