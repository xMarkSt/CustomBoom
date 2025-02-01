using Boom.Business.Services;
using FluentAssertions;

namespace Boom.UnitTests.Services
{
    [TestFixture]
    public class EncryptionServiceTests
    {
        private EncryptionService _encryptionService;

        [SetUp]
        public void Setup()
        {
            _encryptionService = new EncryptionService();
        }

        [Test]
        public void Encrypt_ValidInputs_ReturnsEncryptedString()
        {
            // Arrange
            var payload = "Test payload";
            var key = "12345678901234567890123456789012"; // 32 bytes

            // Act
            var result = _encryptionService.Encrypt(payload, key);

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().Contain(":");
        }

        [Test]
        public void Decrypt_ValidInputs_ReturnsOriginalPayload()
        {
            // Arrange
            var payload = "Test payload";
            var key = "12345678901234567890123456789012"; // 32 bytes
            var encrypted = _encryptionService.Encrypt(payload, key);
            var parts = encrypted.Split(':');
            var iv = parts[0];
            var encryptedPayload = parts[1];

            // Act
            var decrypted = _encryptionService.Decrypt(encryptedPayload, key, iv);

            // Assert
            decrypted.Should().Be(payload);
        }

        [Test]
        public void GenerateSecretKey_ReturnsValidSecretKey()
        {
            // Act
            var secretKey = _encryptionService.GenerateSecretKey();
            // Assert
            secretKey.Should().NotBeNullOrEmpty();
            secretKey.Should().StartWith("__");
            secretKey.Should().Contain(".");
            secretKey.Length.Should().Be(25); // 2 underscores + 14 hex chars + 1 dot + 8 digits
        }

        [Test]
        public void GetDecryptionKey_ReturnsValidDecryptionKey()
        {
            // Arrange
            var secretKey = "__mySecretKey.89012345";

            // Act
            var decryptionKey = _encryptionService.GetDecryptionKey(secretKey);

            // Assert
            decryptionKey.Should().NotBeNullOrEmpty();
            decryptionKey.Length.Should().Be(32); // MD5 hash is 32 characters in hexadecimal
            decryptionKey.Should().Be("818a15c90aa7ad2ca09ec16f0ff7ea80");
        }
    }
}