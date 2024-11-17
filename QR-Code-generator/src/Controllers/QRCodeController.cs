/*using QRCodeGeneratorApp.Models;
using System.Drawing;

namespace QRCodeGeneratorApp.Controller
{
    public class QRCodeController
    {
        private QRCodeModel _qrCodeModel;

        public QRCodeController(string data)
        {
            _qrCodeModel = new QRCodeModel(data);
        }

        // Метод для получения обычного QR-кода
        public Bitmap CreateQRCode()
        {
            return _qrCodeModel.GenerateQRCode();
        }

        // Метод для получения QR-кода с логотипом по центру
        public Bitmap CreateQRCodeWithLogo(string logoPath)
        {
            return _qrCodeModel.GenerateQRCodeWithLogo(logoPath);
        }
    }
}
*/