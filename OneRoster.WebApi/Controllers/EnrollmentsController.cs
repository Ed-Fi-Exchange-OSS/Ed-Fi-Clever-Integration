﻿using System.Threading.Tasks;
using EdFi.OneRoster.WebApi.Helpers;
using EdFi.OneRoster.WebApi.Security;
using EdFi.OneRoster.WebApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EdFi.OneRoster.WebApi.Controllers
{
    [Authorize(AuthenticationSchemes = OAuth1Defaults.AuthenticationScheme + ", " + JwtBearerDefaults.AuthenticationScheme)]
    [Route("[controller]")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        private readonly IOneRosterService _service;

        public EnrollmentsController(IOneRosterService service)
        {
            _service = service;
        }

        [HttpGet]        
        public async Task<IActionResult> GetEnrollments([FromQuery] RequestModel request)
        {
            var response = await _service.GetEnrollmentsAsync(request);
            Response.Headers.Add("X-Total-Count", response.TotalCount.ToString());
            return Ok(new { enrollments = response.Response });
        }
    }
}
