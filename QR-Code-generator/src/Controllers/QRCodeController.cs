using System.Drawing;
using System.IO;
using QRCoder;
using QRCodeGeneratorApp.Models;

namespace QRCodeGeneratorApp.Controller
{
    public class QRCodeController
    {
        private readonly QRCodeModel _qrCodeModel;

        // Constructor to initialize the QRCodeController with the content
        public QRCodeController(string content)
        {
            _qrCodeModel = new QRCodeModel(content);
        }

        // Method to set the error correction level
        public void SetErrorCorrectionLevel(QRCodeGenerator.ECCLevel level)
        {
            _qrCodeModel.ErrorCorrectionLevel = level;
        }

        // Method to set the pixel size of each QR code module
        public void SetPixelsPerModule(int pixelsPerModule)
        {
            _qrCodeModel.PixelsPerModule = pixelsPerModule;
        }

        // Method to generate and retrieve the QR code as a Bitmap
        public Bitmap GetQRCodeImage()
        {
            return _qrCodeModel.GenerateQRCode();
        }

        // Method to save the QR code image to a specified file path and format
        public void SaveQRCodeToFile(string filePath, string format = "png")
        {
            _qrCodeModel.SaveQRCode(filePath, format);
        }

        // Method to save the QR code image with a logo
        public void SaveQRCodeWithLogo(string filePath, string logoPath, int logoSize = 50, string format = "png")
        {
            _qrCodeModel.SaveQRCodeWithLogo(filePath, logoPath, logoSize, format);
        }

        // Method to retrieve the QR code as a byte array
        public byte[] GetQRCodeAsBytes(string format = "png")
        {
            return _qrCodeModel.GetQRCodeBytes(format);
        }
    }
}
