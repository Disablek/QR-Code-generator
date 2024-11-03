using System;
using System.Drawing;
using System.IO;
using QRCoder;

namespace QRCodeGeneratorApp.Models
{
    public class QRCodeModel
    {
        // Свойства для хранения данных QR-кода
        public string Content { get; set; } // Ссылка
        public int PixelsPerModule { get; set; } = 20; // Размер qr-а
        public QRCodeGenerator.ECCLevel ErrorCorrectionLevel { get; set; } = QRCodeGenerator.ECCLevel.Q; // Коррекционный уровень

        // Конструктор класса
        public QRCodeModel(string content)
        {
            Content = content;
        }

        // Метод для генерации QR-кода
        public Bitmap GenerateQRCode()
        {
            using (var qrGenerator = new QRCodeGenerator())
            {
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(Content, ErrorCorrectionLevel);
                using (var qrCode = new QRCode(qrCodeData))
                {
                    Bitmap qrCodeImage = qrCode.GetGraphic(PixelsPerModule);
                    return qrCodeImage;
                }
            }
        }

        // Метод для сохранения QR-кода в файл
        public void SaveQRCode(string filePath, string format = "png")
        {
            Bitmap qrCodeImage = GenerateQRCode();
            ImageFormat imageFormat;

            switch (format.ToLower())
            {
                case "jpeg":
                case "jpg":
                    imageFormat = System.Drawing.Imaging.ImageFormat.Jpeg;
                    break;
                case "bmp":
                    imageFormat = System.Drawing.Imaging.ImageFormat.Bmp;
                    break;
                default:
                    imageFormat = System.Drawing.Imaging.ImageFormat.Png;
                    break;
            }

            qrCodeImage.Save(filePath, imageFormat);
            qrCodeImage.Dispose();
        }

        // Метод для сохранения QR-кода в виде массива байтов
        public byte[] GetQRCodeBytes(string format = "png")
        {
            using (var memoryStream = new MemoryStream())
            {
                Bitmap qrCodeImage = GenerateQRCode();
                ImageFormat imageFormat = format.ToLower() == "jpeg" ? System.Drawing.Imaging.ImageFormat.Jpeg : System.Drawing.Imaging.ImageFormat.Png;

                qrCodeImage.Save(memoryStream, imageFormat);
                qrCodeImage.Dispose();
                return memoryStream.ToArray();
            }
        }
    }
}
