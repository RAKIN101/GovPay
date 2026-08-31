using System.Security.Cryptography;
using System.Text;

namespace GovPay.Cryptography.ECC;

public class EccKeyGenerator
{
    public (string PublicKey, string PrivateKey) GenerateKeyPair()
    {
        using var ecc = ECDsa.Create(ECCurve.NamedCurves.nistP256);

        var publicKey = ecc.ExportSubjectPublicKeyInfo();
        var privateKey = ecc.ExportPkcs8PrivateKey();

        return (
            Convert.ToBase64String(publicKey),
            Convert.ToBase64String(privateKey)
        );
    }

    public byte[] SignData(string data, string privateKeyBase64)
    {
        var privateKeyBytes = Convert.FromBase64String(privateKeyBase64);
        using var ecc = ECDsa.Create();
        ecc.ImportPkcs8PrivateKey(privateKeyBytes, out _);

        var payload = Encoding.UTF8.GetBytes(data);
        return ecc.SignData(payload, HashAlgorithmName.SHA256);
    }

    public bool VerifySignature(string data, byte[] signature, string publicKeyBase64)
    {
        var publicKeyBytes = Convert.FromBase64String(publicKeyBase64);
        using var ecc = ECDsa.Create();
        ecc.ImportSubjectPublicKeyInfo(publicKeyBytes, out _);

        var payload = Encoding.UTF8.GetBytes(data);
        return ecc.VerifyData(payload, signature, HashAlgorithmName.SHA256);
    }
}
