namespace Boom.Business.Services;

public interface IEncryptionService
{
    string Encrypt(string payload, string key);
    string Decrypt(string encryptedPayload, string key, string ivBase64);
    string GenerateSecretKey();
    string GetDecryptionKey(string secretKey);
}