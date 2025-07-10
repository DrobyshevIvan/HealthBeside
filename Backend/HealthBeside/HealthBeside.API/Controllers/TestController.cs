using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthBeside.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class TestController
{
    [HttpGet]
    [Authorize(Roles = "Admin, Doctor, Patient")]
    public async Task<IActionResult> Get()
    {
        return new OkObjectResult("Test endpoint is working!");
    }
}