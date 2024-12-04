using QRCodeGeneratorApp.Models;
using QRCodeGeneratorApp.Data; // DbContext
using ZXing;
using ZXing.Rendering;
using ZXing.Common;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using ZXing.Windows.Compatibility;

namespace QRCodeGeneratorApp.Services
{
    public class QRCodeService
    {
        private readonly ApplicationDbContext _dbContext;

        public QRCodeService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Bitmap GenerateQRCode(string data, Color qrColor, Color backgroundColor)
        {
            var writer = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new EncodingOptions
                {
                    Width = 300,
                    Height = 300,
                    Margin = 0,
                    PureBarcode = true
                },
                Renderer = new BitmapRenderer { Foreground = qrColor, Background = backgroundColor }
            };

            return writer.Write(data);
        }

        public void SaveQRCodeToDatabase(string data, Bitmap qrImage)
        {
            // Конвертируем Bitmap в массив байтов
            byte[] qrImageBytes;
            using (var ms = new MemoryStream())
            {
                qrImage.Save(ms, ImageFormat.Png);
                qrImageBytes = ms.ToArray();
            }

            // Создаем новую запись
            var qrCodeEntity = new QRCodeEntity
            {
                Data = data,
                QRImage = qrImageBytes
            };

            // Сохраняем в базе данных
            _dbContext.QRCodes.Add(qrCodeEntity);
            _dbContext.SaveChanges();
        }
    }
}
