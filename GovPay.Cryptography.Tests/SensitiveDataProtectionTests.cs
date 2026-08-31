using GovPay.Cryptography.Keymanagement;

namespace GovPay.Cryptography.Tests;

public class SensitiveDataProtectionTests
{
    [Fact]
    public void SensitiveDataProtectionService_EncryptsAndDecryptsViaRsa()
    {
        var service = new SensitiveDataProtectionService("govpay-secret-key");
        var (publicKey, privateKey) = service.GenerateRsaKeyPair();

        const string payload = "alice@example.com";
        var encrypted = service.EncryptAsymmetric(payload, publicKey);
        var decrypted = service.DecryptAsymmetric(encrypted, privateKey);

        Assert.Equal(payload, decrypted);
    }

    [Fact]
    public void SensitiveDataProtectionService_SignsAndVerifiesWithEcc()
    {
        var service = new SensitiveDataProtectionService("govpay-secret-key");
        var (publicKey, privateKey) = service.GenerateEccKeyPair();

        const string payload = "user profile data";
        var signature = service.Sign(payload, privateKey);

        var valid = service.VerifySignature(payload, signature, publicKey);

        Assert.True(valid);
    }

    [Fact]
    public void SensitiveDataProtectionService_DetectsTamperingWithMac()
    {
        var service = new SensitiveDataProtectionService("govpay-secret-key");

        const string payload = "amount=500";
        var mac = service.ComputeMac(payload);

        Assert.True(service.VerifyMac(payload, mac));
        Assert.False(service.VerifyMac("amount=5000", mac));
    }
}
