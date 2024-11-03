using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using QRCoder;

namespace QRCodeGeneratorApp.Models
{
    public class QRCodeModel
    {
        public string Content { get; set; }
        public int PixelsPerModule { get; set; } = 20;
        public QRCodeGenerator.ECCLevel ErrorCorrectionLevel { get; set; } = QRCodeGenerator.ECCLevel.Q;

        public QRCodeModel(string content)
        {
            Content = content;
        }

        public Bitmap GenerateQRCode()
        {
            using (var qrGenerator = new QRCodeGenerator())
            {
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(Content, ErrorCorrectionLevel);
                using (var base64QRCode = new Base64QRCode(qrCodeData))
                {
                    string base64Image = base64QRCode.GetGraphic(PixelsPerModule);
                    byte[] imageBytes = Convert.FromBase64String(base64Image);
                    using (var ms = new MemoryStream(imageBytes))
                    {
                        // Create the QR code bitmap and ensure it's in a suitable format
                        var bitmap = new Bitmap(ms);
                        return new Bitmap(bitmap); // This should create a 32bpp image by default
                    }
                }
            }
        }

        public void SaveQRCodeWithLogo(string filePath, string logoPath, int logoSize = 50, string format = "png")
        {
            Bitmap qrCodeImage = GenerateQRCode();

            // Load the logo image and convert it to a suitable format
            using (var logo = new Bitmap(logoPath))
            {
                Bitmap convertedLogo = ConvertLogoTo32Bpp(logo);

                // Resize the logo to fit in the QR code
                Bitmap resizedLogo = new Bitmap(convertedLogo, new Size(logoSize, logoSize));

                // Calculate position to place the logo in the center
                int x = (qrCodeImage.Width - resizedLogo.Width) / 2;
                int y = (qrCodeImage.Height - resizedLogo.Height) / 2;

                // Draw the logo on the QR code
                using (Graphics graphics = Graphics.FromImage(qrCodeImage))
                {
                    graphics.DrawImage(resizedLogo, x, y, resizedLogo.Width, resizedLogo.Height);
                }
            }

            // Determine the image format to save
            ImageFormat imageFormat;
            switch (format.ToLower())
            {
                case "jpeg":
                case "jpg":
                    imageFormat = ImageFormat.Jpeg;
                    break;
                case "bmp":
                    imageFormat = ImageFormat.Bmp;
                    break;
                default:
                    imageFormat = ImageFormat.Png;
                    break;
            }

            // Save the QR code image with logo
            qrCodeImage.Save(filePath, imageFormat);
        }

        private Bitmap ConvertLogoTo32Bpp(Bitmap logo)
        {
            // Check if the logo needs to be converted to a non-indexed format
            if (logo.PixelFormat == PixelFormat.Format1bppIndexed ||
                logo.PixelFormat == PixelFormat.Format4bppIndexed ||
                logo.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                // Create a new bitmap with a non-indexed pixel format
                Bitmap convertedLogo = new Bitmap(logo.Width, logo.Height, PixelFormat.Format32bppArgb);
                using (Graphics g = Graphics.FromImage(convertedLogo))
                {
                    g.Clear(Color.Transparent); // Clear with transparent background
                    g.DrawImage(logo, 0, 0); // Draw the logo onto the new bitmap
                }
                return convertedLogo;
            }
            return new Bitmap(logo); // Return the original bitmap if it's already suitable
        }

        public void SaveQRCode(string filePath, string format = "png")
        {
            Bitmap qrCodeImage = GenerateQRCode();
            ImageFormat imageFormat;
            switch (format.ToLower())
            {
                case "jpeg":
                case "jpg":
                    imageFormat = ImageFormat.Jpeg;
                    break;
                case "bmp":
                    imageFormat = ImageFormat.Bmp;
                    break;
                default:
                    imageFormat = ImageFormat.Png;
                    break;
            }
            qrCodeImage.Save(filePath, imageFormat);
            qrCodeImage.Dispose();
        }

        public byte[] GetQRCodeBytes(string format = "png")
        {
            using (var memoryStream = new MemoryStream())
            {
                Bitmap qrCodeImage = GenerateQRCode();
                ImageFormat imageFormat = format.ToLower() == "jpeg" ? ImageFormat.Jpeg : ImageFormat.Png;
                qrCodeImage.Save(memoryStream, imageFormat);
                qrCodeImage.Dispose();
                return memoryStream.ToArray();
            }
        }
    }
}
