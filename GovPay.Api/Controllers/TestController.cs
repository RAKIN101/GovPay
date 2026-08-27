using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GovPay.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    [HttpGet("public")]
    public IActionResult Public()
    {
        return Ok(new
        {
            message = "Anyone can access this."
        });
    }

    [Authorize]
    [HttpGet("protected")]
    public IActionResult Protected()
    {
        return Ok(new
        {
            message = "You are authenticated.",
            username = User.Identity?.Name
        });
    }

    [Authorize(Roles = "Citizen")]
    [HttpGet("citizen")]
    public IActionResult Citizen()
    {
        return Ok(new
        {
            message = "Citizen access granted.",
            username = User.Identity?.Name
        });
    }
}