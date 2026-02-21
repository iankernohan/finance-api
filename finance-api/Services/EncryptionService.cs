using System.Security.Cryptography;
using System.Text;

namespace finance_api.Services;

public class EncryptionService
{
    private readonly byte[] _key;

    public EncryptionService(IConfiguration configuration)
    {
        var base64Key = configuration["ENCRYPTION_KEY"];
        if (base64Key == null) throw new Exception("No Encryption Key Found.");
        _key = Convert.FromBase64String(base64Key);
        if (_key.Length != 32) throw new Exception("Encryption Key Must Be 256 bits (32 bytes).");
    }

    public string Encrypt(string plainText)
    {
        var plainTextBytes = Encoding.UTF8.GetBytes(plainText);

        var nonce = RandomNumberGenerator.GetBytes(12);
        var cipherText = new byte[plainTextBytes.Length];
        var tag = new byte[16];

        using var aesGcm = new AesGcm(_key, tag.Length);

        aesGcm.Encrypt(nonce, plainTextBytes, cipherText, tag);

        var result = new byte[nonce.Length + tag.Length + cipherText.Length];

        Buffer.BlockCopy(nonce, 0, result, 0, nonce.Length);
        Buffer.BlockCopy(tag, 0, result, nonce.Length, tag.Length);
        Buffer.BlockCopy(cipherText, 0, result, nonce.Length + tag.Length, cipherText.Length);

        return Convert.ToBase64String(result);
    }

    public string Decrypt(string encrypted)
    {
        var fullCipher = Convert.FromBase64String(encrypted);

        var nonce = new byte[12];
        var tag = new byte[16];
        var cipherText = new byte[fullCipher.Length - nonce.Length - tag.Length];

        Buffer.BlockCopy(fullCipher, 0, nonce, 0, nonce.Length);
        Buffer.BlockCopy(fullCipher, nonce.Length, tag, 0, tag.Length);
        Buffer.BlockCopy(fullCipher, nonce.Length + tag.Length, cipherText, 0, cipherText.Length);

        using var aesGcm = new AesGcm(_key, tag.Length);

        var plainTextBytes = new byte[cipherText.Length];

        aesGcm.Decrypt(nonce, cipherText, tag, plainTextBytes);

        return Encoding.UTF8.GetString(plainTextBytes);
    }
}
