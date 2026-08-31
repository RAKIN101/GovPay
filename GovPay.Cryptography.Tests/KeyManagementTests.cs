using GovPay.Cryptography.ECC;
using GovPay.Cryptography.HMAC;
using GovPay.Cryptography.Keymanagement;
using GovPay.Cryptography.RSA;

namespace GovPay.Cryptography.Tests;

public class KeyManagementTests
{
    [Fact]
    public void RsaKeyPair_AllowsEncryptionAndDecryption()
    {
        var generator = new RsaKeyGenerator();
        var (publicKey, privateKey) = generator.GenerateKeyPair();

        const string original = "SensitiveGovPayData";
        var encrypted = generator.Encrypt(original, publicKey);
        var decrypted = generator.Decrypt(encrypted, privateKey);

        Assert.Equal(original, decrypted);
    }

    [Fact]
    public void EccKeyPair_AllowsSignatureVerification()
    {
        var generator = new EccKeyGenerator();
        var (publicKey, privateKey) = generator.GenerateKeyPair();

        const string message = "GovPay payment record";
        var signature = generator.SignData(message, privateKey);

        var isValid = generator.VerifySignature(message, signature, publicKey);

        Assert.True(isValid);
    }

    [Fact]
    public void HmacIntegrityService_DetectsTampering()
    {
        var service = new HmacIntegrityService("GovPay-super-secret-key");
        const string payload = "amount=500";

        var mac = service.ComputeMac(payload);
        var isValid = service.VerifyMac(payload, mac);
        var tampered = service.VerifyMac("amount=5000", mac);

        Assert.True(isValid);
        Assert.False(tampered);
    }

    [Fact]
    public void KeyManagementService_ProvidesExpectedCryptoWorkflow()
    {
        var service = new KeyManagementService();
        var (rsaPublic, rsaPrivate) = service.GenerateRsaKeyPair();
        var (eccPublic, eccPrivate) = service.GenerateEccKeyPair();

        const string message = "Citizen payment info";
        var encrypted = service.EncryptWithRsa(message, rsaPublic);
        var decrypted = service.DecryptWithRsa(encrypted, rsaPrivate);
        var signature = service.SignWithEcc(message, eccPrivate);
        var valid = service.VerifyEccSignature(message, signature, eccPublic);

        Assert.Equal(message, decrypted);
        Assert.True(valid);
    }
}
