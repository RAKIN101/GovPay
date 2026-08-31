using GovPay.Cryptography.ECC;
using GovPay.Cryptography.RSA;

namespace GovPay.Cryptography.Keymanagement;

public class KeyManagementService
{
    private readonly RsaKeyGenerator _rsaKeyGenerator = new();
    private readonly EccKeyGenerator _eccKeyGenerator = new();

    public (string PublicKey, string PrivateKey) GenerateRsaKeyPair()
    {
        return _rsaKeyGenerator.GenerateKeyPair();
    }

    public (string PublicKey, string PrivateKey) GenerateEccKeyPair()
    {
        return _eccKeyGenerator.GenerateKeyPair();
    }

    public byte[] EncryptWithRsa(string plainText, string publicKeyBase64)
    {
        return _rsaKeyGenerator.Encrypt(plainText, publicKeyBase64);
    }

    public string DecryptWithRsa(byte[] encryptedData, string privateKeyBase64)
    {
        return _rsaKeyGenerator.Decrypt(encryptedData, privateKeyBase64);
    }

    public byte[] SignWithEcc(string data, string privateKeyBase64)
    {
        return _eccKeyGenerator.SignData(data, privateKeyBase64);
    }

    public bool VerifyEccSignature(string data, byte[] signature, string publicKeyBase64)
    {
        return _eccKeyGenerator.VerifySignature(data, signature, publicKeyBase64);
    }
}
