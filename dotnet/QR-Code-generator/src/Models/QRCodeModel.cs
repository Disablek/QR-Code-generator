using ZXing;
using ZXing.Rendering;
using ZXing.Common;
using ZXing.QrCode.Internal;
using SkiaSharp;
using ZXing.Windows.Compatibility;

namespace QRCodeGeneratorApp.Models{
    public class QRCodeModel
    {
        public string Data { get; set; }

        public QRCodeModel(string data)
        {
            Data = data;
        }

        // Метод для создания QR-кода с логотипом
        public void CreateQRCodeWithLogo(string filePath)
        {
            BarcodeWriter barcodeWriter = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new EncodingOptions
                {
                    Width = 300,
                    Height = 300,
                    Margin = 0,
                    PureBarcode = false,
                    Hints = { [EncodeHintType.ERROR_CORRECTION] = ErrorCorrectionLevel.H }
                }
            };

            // Генерация QR-кода как BitMatrix
            BitMatrix matrix = barcodeWriter.Encode(Data);

            // Использование SkiaSharp для создания изображения
            SKBitmap skBitmap = new SKBitmap(matrix.Width, matrix.Height);
            using (SKCanvas canvas = new SKCanvas(skBitmap))
            {
                canvas.Clear(SKColors.White);
                SKPaint blackPaint = new SKPaint { Color = SKColors.Black };

                for (int y = 0; y < matrix.Height; y++)
                {
                    for (int x = 0; x < matrix.Width; x++)
                    {
                        if (matrix[x, y]) // Если пиксель черный, рисуем его
                        {
                            canvas.DrawPoint(x, y, blackPaint);
                        }
                    }
                }
            }

            // Работа с логотипом
            SKBitmap logo = SKBitmap.Decode(@"C:\Users\Disable\Pictures\Roblox\RobloxScreenShot20240802_012422873.png");
            if (logo == null)
            {
                Console.WriteLine("Ошибка: логотип не удалось загрузить. Проверьте путь к файлу.");
                return;
            }
            int logoSize = Math.Min(skBitmap.Width, skBitmap.Height) / 5;
            SKBitmap resizedLogo = logo.Resize(new SKImageInfo(logoSize, logoSize), SKFilterQuality.High);

            using (SKCanvas canvas = new SKCanvas(skBitmap))
            {
                canvas.DrawBitmap(resizedLogo, new SKPoint((skBitmap.Width - resizedLogo.Width) / 2, (skBitmap.Height - resizedLogo.Height) / 2));
            }

            // Сохранение изображения с QR-кодом и логотипом
            using (var stream = System.IO.File.OpenWrite(filePath))
            {
                skBitmap.Encode(SKEncodedImageFormat.Png, 100).SaveTo(stream);
            }

            Console.WriteLine("QR код с логотипом успешно создан и сохранен по адресу: " + filePath);
        }

        // Метод для создания обычного QR-кода без логотипа
        public void CreateQRCode(string filePath)
        {
            BarcodeWriter barcodeWriter = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new EncodingOptions
                {
                    Width = 300,
                    Height = 300,
                    Margin = 0,
                    PureBarcode = true,
                    Hints = { [EncodeHintType.ERROR_CORRECTION] = ErrorCorrectionLevel.H }
                }
            };

            // Генерация QR-кода как BitMatrix
            BitMatrix matrix = barcodeWriter.Encode(Data);

            // Использование SkiaSharp для создания изображения
            SKBitmap skBitmap = new SKBitmap(matrix.Width, matrix.Height);
            using (SKCanvas canvas = new SKCanvas(skBitmap))
            {
                canvas.Clear(SKColors.White);
                SKPaint blackPaint = new SKPaint { Color = SKColors.Black };

                for (int y = 0; y < matrix.Height; y++)
                {
                    for (int x = 0; x < matrix.Width; x++)
                    {
                        if (matrix[x, y]) // Если пиксель черный, рисуем его
                        {
                            canvas.DrawPoint(x, y, blackPaint);
                        }
                    }
                }
            }

            // Сохранение изображения с QR-кодом
            using (var stream = System.IO.File.OpenWrite(filePath))
            {
                skBitmap.Encode(SKEncodedImageFormat.Png, 100).SaveTo(stream);
            }

            Console.WriteLine("Обычный QR код успешно создан и сохранен по адресу: " + filePath);
        }
    }
}