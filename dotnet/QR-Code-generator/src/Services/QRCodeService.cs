using QRCodeGeneratorApp.Models;
using QRCodeGeneratorApp.Data; // DbContext
using ZXing;
using SkiaSharp;
using SkiaSharp.QrCode;
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
        private const string DbPath = "QRCodesDatabase.db";

        public QRCodeService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Bitmap GenerateQRCode(string data)
        {
            // Устанавливаем фиксированные цвета (например, черный QR-код на белом фоне)
            var qrColor = Color.Black;  // Черный цвет для QR-кода
            var backgroundColor = Color.White;  // Белый цвет для фона

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

     public IEnumerable<QRCodeHistoryItem> GetQRCodeHistory(int lastLoadedId, int take)
    {
        Console.WriteLine($"Получен запрос: lastLoadedId = {lastLoadedId}, take = {take}");

        var qrCodes = _dbContext.QRCodes
            .Where(q => lastLoadedId == 0 || q.Id <= lastLoadedId) // Включаем запись с lastLoadedId
            .OrderByDescending(q => q.Id)
            .Take(take)
            .Select(q => new QRCodeHistoryItem
            {
                Id = q.Id,
                Data = q.Data,
                CreatedAt = q.CreatedAt,
                QRImageBase64 = Convert.ToBase64String(q.QRImage)
            })
            .ToList();

        Console.WriteLine($"Возвращаю записи: {string.Join(", ", qrCodes.Select(q => q.Id))}");
        return qrCodes;
    }


    }
    // DTO-класс для истории QR-кодов
    public class QRCodeHistoryItem
    {
        public int Id { get; set; }
        public string Data { get; set; }
        public DateTime CreatedAt { get; set; }
        public string QRImageBase64 { get; set; } // Изображение QR-кода в формате Base64
    }

}
