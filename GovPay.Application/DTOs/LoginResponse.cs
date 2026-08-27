namespace GovPay.Application.DTOs;

public class LoginResponse
{
    public int Id { get; set; }

    public string Username { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    public bool RequiresTwoFactor { get; set; }

    public string? Token { get; set; }
}