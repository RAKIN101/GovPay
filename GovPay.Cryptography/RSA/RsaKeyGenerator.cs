using System.Security.Cryptography;
using System.Text;

namespace GovPay.Cryptography.RSA;

public class RsaKeyGenerator
{
    public (string PublicKeyXml, string PrivateKeyXml) GenerateKeyPair()
    {
        using var rsa = System.Security.Cryptography.RSA.Create(2048);

        var publicKey = rsa.ExportRSAPublicKey();
        var privateKey = rsa.ExportRSAPrivateKey();

        return (
            Convert.ToBase64String(publicKey),
            Convert.ToBase64String(privateKey)
        );
    }

    public byte[] Encrypt(string plainText, string publicKeyBase64)
    {
        var publicKeyBytes = Convert.FromBase64String(publicKeyBase64);
        using var rsa = System.Security.Cryptography.RSA.Create();
        rsa.ImportRSAPublicKey(publicKeyBytes, out _);

        var data = Encoding.UTF8.GetBytes(plainText);
        return rsa.Encrypt(data, RSAEncryptionPadding.OaepSHA256);
    }

    public string Decrypt(byte[] encrypted, string privateKeyBase64)
    {
        var privateKeyBytes = Convert.FromBase64String(privateKeyBase64);
        using var rsa = System.Security.Cryptography.RSA.Create();
        rsa.ImportRSAPrivateKey(privateKeyBytes, out _);

        var decrypted = rsa.Decrypt(encrypted, RSAEncryptionPadding.OaepSHA256);
        return Encoding.UTF8.GetString(decrypted);
    }
}
