using GovPay.Application.DTOs;
using GovPay.Application.Services;
using GovPay.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GovPay.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProfileController : ControllerBase
{
    private readonly UserProfileService _profileService;

    public ProfileController(UserProfileService profileService)
    {
        _profileService = profileService;
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var profile = await _profileService.GetProfileAsync(userId);
        if (profile is null) return NotFound();

        return Ok(new
        {
            profile.Id,
            profile.Username,
            profile.Email,
            profile.Role
        });
    }

    [Authorize]
    [HttpPut]
    public async Task<IActionResult> UpdateProfile([FromBody] ProfileRequest request)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var profile = await _profileService.UpdateProfileAsync(userId, request);
        if (profile is null) return NotFound();

        return Ok(new
        {
            profile.Id,
            profile.Username,
            profile.Email,
            profile.Role
        });
    }
}
