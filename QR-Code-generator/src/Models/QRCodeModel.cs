using System.Drawing;
using System.Drawing.Imaging;
using ZXing;
using ZXing.Common;
using ZXing.Windows.Compatibility;
using ZXing.QrCode.Internal;
using System.Windows.Forms;


namespace QRCodeGeneratorApp.Models
{
    public class QRCodeModel
    {
        public string Data { get; set; }  // Данные для кодирования в QR

        public QRCodeModel(string data)
        {
            Data = data;
        }

        public void XUI(string filePath)
        {
            // Инициализация BarcodeWriter для генерации QR-кода
            BarcodeWriter barcodeWriter = new BarcodeWriter();
            EncodingOptions encodingOptions = new EncodingOptions()
            {
                Width = 300,
                Height = 300,
                Margin = 0,
                PureBarcode = false
            };
            encodingOptions.Hints.Add(EncodeHintType.ERROR_CORRECTION, ErrorCorrectionLevel.H);
            barcodeWriter.Renderer = new BitmapRenderer();
            barcodeWriter.Options = encodingOptions;
            barcodeWriter.Format = BarcodeFormat.QR_CODE;

            // Генерация QR-кода
            Bitmap bitmap = barcodeWriter.Write(Data);

            // Загрузка логотипа
            Bitmap logo = new Bitmap(@"C:\Users\Disable\Pictures\Roblox\RobloxScreenShot20240802_012422873.png");

            // Убедитесь, что логотип не больше QR-кода
            int logoSize = Math.Min(bitmap.Width, bitmap.Height) / 5; // Логотип 1/5 от размера QR
            Bitmap resizedLogo = new Bitmap(logo, new Size(logoSize, logoSize));

            // Вставка логотипа в центр QR-кода
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.DrawImage(resizedLogo, new Point((bitmap.Width - resizedLogo.Width) / 2, (bitmap.Height - resizedLogo.Height) / 2));
            }


            bitmap.Save(filePath, ImageFormat.Png);

            // Освобождение ресурсов
            logo.Dispose();
            bitmap.Dispose();

            Console.WriteLine("QR код с логотипом успешно создан и сохранен по адресу: " + filePath);
        }

    }
}   
