using System.Security.Cryptography;

namespace GovPay.Cryptography.Hashing;

public class PasswordHasher
{
    public (string Hash, string Salt) HashPassword(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(16);

        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            100_000,
            HashAlgorithmName.SHA256,
            32);

        return (
            Convert.ToBase64String(hash),
            Convert.ToBase64String(salt)
        );
    }

    public bool VerifyPassword(
        string password,
        string storedHash,
        string storedSalt)
    {
        byte[] salt = Convert.FromBase64String(storedSalt);

        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            100_000,
            HashAlgorithmName.SHA256,
            32);

        return CryptographicOperations.FixedTimeEquals(
            hash,
            Convert.FromBase64String(storedHash));
    }
}