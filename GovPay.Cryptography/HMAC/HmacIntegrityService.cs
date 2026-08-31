using System.Security.Cryptography;
using System.Text;

namespace GovPay.Cryptography.HMAC;

public class HmacIntegrityService
{
    private readonly byte[] _secretKey;

    public HmacIntegrityService(string secretKey)
    {
        _secretKey = Encoding.UTF8.GetBytes(secretKey);
    }

    public string ComputeMac(string payload)
    {
        var payloadBytes = Encoding.UTF8.GetBytes(payload);
        using var hmac = new HMACSHA256(_secretKey);
        return Convert.ToBase64String(hmac.ComputeHash(payloadBytes));
    }

    public bool VerifyMac(string payload, string expectedMac)
    {
        var actual = ComputeMac(payload);
        return CryptographicOperations.FixedTimeEquals(
            Convert.FromBase64String(actual),
            Convert.FromBase64String(expectedMac));
    }
}
