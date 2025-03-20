using QRCoder;
using System.Windows.Media.Imaging;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace Ayalon.MFA.Core
{
    public class QrCodeGenerator
    {
        public Bitmap GenerateQrCode(string secretKey, string user, string issuer)
        {
            var totpUri = $"otpauth://totp/{issuer}:{user}?secret={secretKey}&issuer={issuer}&digits=6";
            var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(totpUri, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new QRCode(qrCodeData);
            return qrCode.GetGraphic(4);
        }
        
        public BitmapImage ConvertBitmapToBitmapImage(Bitmap bitmap)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                bitmap.Save(ms, ImageFormat.Bmp);
                ms.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = ms;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                return bitmapImage;
            }
        }
    }
}
