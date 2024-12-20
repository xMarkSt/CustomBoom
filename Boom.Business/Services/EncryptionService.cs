using System.Security.Cryptography;
using System.Text;

namespace Boom.Business.Services;

public class EncryptionService
{
    public string Encrypt(string payload, string key)
    {
        try
        {
            // Generate a random IV (16 bytes)
            using var aes = Aes.Create();
            aes.KeySize = 256;
            aes.BlockSize = 128;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
                
            // Convert the key to a byte array
            var keyBytes = Encoding.UTF8.GetBytes(key);
            if (keyBytes.Length != 32)
            {
                throw new ArgumentException("The key must be 32 bytes for AES-256.");
            }

            aes.Key = keyBytes;

            // Generate a random IV
            aes.GenerateIV();
            var iv = aes.IV;

            // Encrypt the payload
            using var encryptor = aes.CreateEncryptor();
            var payloadBytes = Encoding.UTF8.GetBytes(payload);
            var encryptedBytes = encryptor.TransformFinalBlock(payloadBytes, 0, payloadBytes.Length);

            // Combine IV and encrypted payload
            var result = Convert.ToBase64String(iv) + ":" + Convert.ToBase64String(encryptedBytes);
            return result;
        }
        catch (Exception ex)
        {
            // Log the error and throw
            // Replace this with your actual logging logic
            Console.Error.WriteLine($"Encryption failed: {ex.Message}");
            throw new Exception("Encryption failed", ex);
        }
    }
    
    public string Decrypt(string encryptedPayload, string key, string ivBase64)
    {
        try
        {
            // Decode the Base64-encoded IV
            var iv = Convert.FromBase64String(ivBase64);

            // Convert the key to a byte array
            var keyBytes = Encoding.UTF8.GetBytes(key);
            if (keyBytes.Length != 32)
            {
                throw new ArgumentException("The key must be 32 bytes for AES-256.");
            }

            // Decode the encrypted payload from Base64
            var encryptedBytes = Convert.FromBase64String(encryptedPayload);

            // Decrypt the payload
            using var aes = Aes.Create();
            if (aes == null)
            {
                throw new InvalidOperationException("Failed to create AES instance.");
            }

            aes.KeySize = 256;
            aes.BlockSize = 128;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            aes.Key = keyBytes;
            aes.IV = iv;

            using var decryptor = aes.CreateDecryptor();
            var decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
            return Encoding.UTF8.GetString(decryptedBytes);
        }
        catch (Exception ex)
        {
            // Log the error and throw
            // Replace this with your actual logging logic
            Console.Error.WriteLine($"Decryption failed: {ex.Message}");
            throw new Exception("Decryption failed", ex);
        }
    }
    
    public static string GenerateSecretKey()
    {
        try
        {
            // Generate 7 random bytes and convert them to a hexadecimal string
            var randomHex = BitConverter.ToString(RandomNumberGenerator.GetBytes(7)).Replace("-", "").ToLower();

            // Generate a random 8-digit number
            var randomNumbers = RandomNumberGenerator.GetInt32(10000000, 100000000);

            // Combine and return the secret key
            return $"__{randomHex}.{randomNumbers}";
        }
        catch (Exception ex)
        {
            // Log the error (replace with your logging logic)
            Console.Error.WriteLine($"Failed to generate secret key: {ex.Message}");
        }
        return string.Empty;
    }
    
    public string GetDecryptionKey(string secretKey)
    {
        var input = $"// + (void)secure:(NSString *)pa{secretKey}";
        var inputBytes = Encoding.UTF8.GetBytes(input);
        var hashBytes = MD5.HashData(inputBytes);
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
    }
}