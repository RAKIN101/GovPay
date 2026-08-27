using GovPay.Application.DTOs;
using GovPay.Application.Interfaces;
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
    private readonly IUserService _userService;

    public AuthController(IUserService userService)
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
        var result = await _userService.LoginAsync(request);

        if (result is null)
        {
            return Unauthorized(new
            {
                message = "Invalid username or password."
            });
        }

        return Ok(result);
    }

    [HttpPost("verify-2fa")]
    public async Task<IActionResult> VerifyTwoFactor(VerifyTwoFactorRequest request)
    {
        var result = await _userService.VerifyTwoFactorAsync(request);

        if (result is null)
        {
            return Unauthorized(new
            {
                message = "Invalid or expired OTP."
            });
        }

        return Ok(result);
    }
}