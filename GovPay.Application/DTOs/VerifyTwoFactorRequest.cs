namespace GovPay.Application.DTOs;

public class VerifyTwoFactorRequest
{
    public string Username { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}