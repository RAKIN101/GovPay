namespace GovPay.Domain.Entities;

public class User
{
    public int Id { get; set; }

    public string Username { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public string PasswordSalt { get; set; } = string.Empty;

    public string Role { get; set; } = "Citizen";

    public bool TwoFactorEnabled { get; set; } = false;
    public string? TwoFactorCodeHash { get; set; }

    public DateTime? TwoFactorCodeExpiresAt { get; set; }
    public string? TwoFactorCodeSalt { get; set; }
}