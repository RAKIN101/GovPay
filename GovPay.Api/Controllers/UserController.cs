using System.Security.Claims;
using GovPay.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GovPay.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();

        return Ok(users.Select(u => new
        {
            u.Id,
            u.Username,
            u.Email,
            u.Role
        }));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _userService.DeleteUserAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        return Ok(new
        {
            user.Id,
            user.Username,
            user.Email,
            user.Role,
            Deleted = true
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(int id)
    {
        var currentUserIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var currentRole = User.FindFirstValue(ClaimTypes.Role);

        if (!string.Equals(currentRole, "Admin", StringComparison.OrdinalIgnoreCase)
            && int.TryParse(currentUserIdClaim, out var currentUserId)
            && currentUserId != id)
        {
            return Forbid();
        }

        var user = await _userService.GetUserByIdAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        return Ok(new
        {
            user.Id,
            user.Username,
            user.Email,
            user.Role
        });
    }
}