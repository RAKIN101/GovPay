using System.Security.Cryptography;
using GovPay.Cryptography.Hashing;

namespace GovPay.Application.Services;

public class TwoFactorService
{
    private readonly PasswordHasher _passwordHasher;

    public TwoFactorService(PasswordHasher passwordHasher)
    {
        _passwordHasher = passwordHasher;
    }

    public (string Code, string Hash, string Salt, DateTime ExpiresAt) GenerateCode()
    {
        var code = RandomNumberGenerator.GetInt32(100000, 1000000);
        
        var codeString = code.ToString();

        var (hash, salt) = _passwordHasher.HashPassword(codeString);

        var expiresAt = DateTime.UtcNow.AddMinutes(5);

        return (codeString, hash, salt, expiresAt);
    }
}