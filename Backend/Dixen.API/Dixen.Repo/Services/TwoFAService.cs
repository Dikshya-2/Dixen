using QRCoder;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Dixen.Repo.Services
{
    public class TwoFAService
    {
        private readonly UrlEncoder _urlEncoder;
        private const string AuthenticatorUriFormat =
            "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";
        public TwoFAService(UrlEncoder urlEncoder)
        {
            _urlEncoder = urlEncoder;
        }
        public string GenerateQrCodeUri(string email, string unformattedKey)
        {
            var issuer = _urlEncoder.Encode("DixenApp");
            var account = _urlEncoder.Encode(email);

            return string.Format(
                CultureInfo.InvariantCulture,
                AuthenticatorUriFormat,
                issuer,
                account,
                unformattedKey);
        }
        public string GenerateQrCodeImage(string email, string secretKey)
        {
            var uri = GenerateQrCodeUri(email, secretKey);
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(uri, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrCodeData); 
            var bytes = qrCode.GetGraphic(20);
            var base64 = Convert.ToBase64String(bytes);
            return $"data:image/png;base64,{base64}";
        }
    }
}
