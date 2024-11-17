using System;
using System.Drawing;
using System.Drawing.Imaging;
using ZXing;
using ZXing.Common;
using ZXing.Windows.Compatibility;
using ZXing.QrCode.Internal;
using Python.Runtime;

namespace QRCodeGeneratorApp.Models
{
    public class QRCodeModel
    {
        public string Data { get; set; }  // Данные для кодирования в QR

        public QRCodeModel(string data)
        {
            Data = data;
        }

        public string GenerateLinkFromImage(string filepath)
        {
            string imageUrl = string.Empty;

            Runtime.PythonDLL = @"C:\Users\Disable\AppData\Local\Programs\Python\Python311\python311.dll";
            PythonEngine.Initialize();

            using (Py.GIL())
            {
                // Добавляем путь к директории с lok.py в sys.path
                dynamic sys = Py.Import("sys");
                sys.path.append(@"C:\Users\Disable\Source\Repos\QR-Code-generator\QR-Code-generator\src\Models");
                // Замените на фактический путь к lok.py

                // Импортируем скрипт lok и вызываем функцию createimgBB
                var pythonscript = Py.Import("lok");
                var message = new PyString(filepath);
                var result = pythonscript.InvokeMethod("createimgBB", new PyObject[] { message });
                imageUrl = result.ToString();
            }

            Console.WriteLine("URL загруженного изображения: " + imageUrl);
            return imageUrl;
        }

        // Метод для создания QR-кода с логотипом
        public void CreateQRCodeWithLogo(string filePath)
        {
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

            Bitmap bitmap = barcodeWriter.Write(Data);

            Bitmap logo = new Bitmap(@"C:\Users\Disable\Pictures\Roblox\RobloxScreenShot20240802_012422873.png");

            int logoSize = Math.Min(bitmap.Width, bitmap.Height) / 5;
            Bitmap resizedLogo = new Bitmap(logo, new Size(logoSize, logoSize));

            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.DrawImage(resizedLogo, new Point((bitmap.Width - resizedLogo.Width) / 2, (bitmap.Height - resizedLogo.Height) / 2));
            }

            bitmap.Save(filePath, ImageFormat.Png);

            logo.Dispose();
            bitmap.Dispose();

            Console.WriteLine("QR код с логотипом успешно создан и сохранен по адресу: " + filePath);
        }

        // Метод для создания обычного QR-кода без логотипа
        public void CreateQRCode(string filePath)
        {
            BarcodeWriter barcodeWriter = new BarcodeWriter();
            EncodingOptions encodingOptions = new EncodingOptions()
            {
                Width = 300,
                Height = 300,
                Margin = 0,
                PureBarcode = true
            };
            encodingOptions.Hints.Add(EncodeHintType.ERROR_CORRECTION, ErrorCorrectionLevel.H);
            barcodeWriter.Renderer = new BitmapRenderer();
            barcodeWriter.Options = encodingOptions;
            barcodeWriter.Format = BarcodeFormat.QR_CODE;

            Bitmap bitmap = barcodeWriter.Write(Data);
            bitmap.Save(filePath, ImageFormat.Png);
            bitmap.Dispose();

            Console.WriteLine("Обычный QR код успешно создан и сохранен по адресу: " + filePath);
        }
    }
}
