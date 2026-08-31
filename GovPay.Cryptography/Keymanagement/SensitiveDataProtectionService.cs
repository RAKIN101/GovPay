using System.Security.Cryptography;
using System.Text;
using GovPay.Cryptography.ECC;
using GovPay.Cryptography.HMAC;
using GovPay.Cryptography.RSA;

namespace GovPay.Cryptography.Keymanagement;

public class SensitiveDataProtectionService
{
    private readonly RsaKeyGenerator _rsaKeyGenerator = new();
    private readonly EccKeyGenerator _eccKeyGenerator = new();
    private readonly HmacIntegrityService _integrityService;

    public SensitiveDataProtectionService(string secretKey)
    {
        _integrityService = new HmacIntegrityService(secretKey);
    }

    public (string PublicKey, string PrivateKey) GenerateRsaKeyPair()
    {
        return _rsaKeyGenerator.GenerateKeyPair();
    }

    public (string PublicKey, string PrivateKey) GenerateEccKeyPair()
    {
        return _eccKeyGenerator.GenerateKeyPair();
    }

    public string EncryptAsymmetric(string plaintext, string publicKeyBase64)
    {
        var publicKeyBytes = Convert.FromBase64String(publicKeyBase64);
        using var rsa = System.Security.Cryptography.RSA.Create();
        rsa.ImportRSAPublicKey(publicKeyBytes, out _);

        var payload = Encoding.UTF8.GetBytes(plaintext);
        var encrypted = rsa.Encrypt(payload, RSAEncryptionPadding.Pkcs1);
        return Convert.ToBase64String(encrypted);
    }

    public string DecryptAsymmetric(string encryptedBase64, string privateKeyBase64)
    {
        var encrypted = Convert.FromBase64String(encryptedBase64);
        var privateKeyBytes = Convert.FromBase64String(privateKeyBase64);
        using var rsa = System.Security.Cryptography.RSA.Create();
        rsa.ImportRSAPrivateKey(privateKeyBytes, out _);

        var decrypted = rsa.Decrypt(encrypted, RSAEncryptionPadding.Pkcs1);
        return Encoding.UTF8.GetString(decrypted);
    }

    public string Sign(string payload, string privateKeyBase64)
    {
        var signature = _eccKeyGenerator.SignData(payload, privateKeyBase64);
        return Convert.ToBase64String(signature);
    }

    public bool VerifySignature(string payload, string signatureBase64, string publicKeyBase64)
    {
        var signature = Convert.FromBase64String(signatureBase64);
        return _eccKeyGenerator.VerifySignature(payload, signature, publicKeyBase64);
    }

    public string ComputeMac(string payload)
    {
        return _integrityService.ComputeMac(payload);
    }

    public bool VerifyMac(string payload, string expectedMac)
    {
        return _integrityService.VerifyMac(payload, expectedMac);
    }
}
