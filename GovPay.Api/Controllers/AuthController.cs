using GovPay.Application.DTOs;
using GovPay.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace GovPay.Api.Controllers;

public interface IAuthController
{
    Task<IActionResult> Login(LoginRequest request);
    Task<IActionResult> Register(RegisterRequest request);
    Task<IActionResult> VerifyTwoFactor(VerifyTwoFactorRequest request);
}

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase, IAuthController
{
    private readonly UserService _userService;

    public AuthController(UserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var user = await _userService.RegisterAsync(request);

        return Ok(new
        {
            user.Id,
            user.Username,
            user.Email,
            user.Role
        });
    }
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await _userService.LoginAsync(request);

        if (user is null)
        {
            return Unauthorized(new
            {
                message = "Invalid username or password."
            });
        }

        return Ok(new
        {
            user.Id,
            user.Username,
            user.Email,
            user.Role
        });
    }

    [HttpPost("verify-2fa")]
    public async Task<IActionResult> VerifyTwoFactor(VerifyTwoFactorRequest request)
    {
        var user = await _userService.VerifyTwoFactorAsync(request);

        if (user is null)
        {
            return Unauthorized(new
            {
                message = "Invalid or expired OTP."
            });
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