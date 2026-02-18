using Dixen.Repo.Services;
using System.Text.Encodings.Web;
using Xunit;

namespace DixenXUnitTest.ServiceTests
{
    public class TwoFAServiceTests
    {
        private readonly TwoFAService _service;
        public TwoFAServiceTests()
        {
            var urlEncoder = UrlEncoder.Default;
            _service = new TwoFAService(urlEncoder);
        }

        [Fact]
        public void GenerateQrCodeUri_ShouldReturnCorrectlyFormattedUri()
        {
            // Arrange
            string email = "user@example.com";
            string key = "SECRETKEY123";
            // Act
            string uri = _service.GenerateQrCodeUri(email, key);
            // Assert
            Assert.Contains("otpauth://totp/", uri);
            Assert.Contains("DixenApp", uri);
            Assert.Contains(email, uri);
            Assert.Contains(key, uri);
        }

        [Fact]
        public void GenerateQrCodeImage_ShouldReturnBase64Png()
        {
            // Arrange
            string email = "user@example.com";
            string key = "SECRETKEY123";
            // Act
            string qrImage = _service.GenerateQrCodeImage(email, key);
            // Assert
            Assert.StartsWith("data:image/png;base64,", qrImage);
            Assert.True(qrImage.Length > 100, "QR code image base64 is unexpectedly short.");
        }
    }
}
