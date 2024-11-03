using System;
using System.Drawing;
using QRCodeGeneratorApp.Controller;
using QRCoder;

namespace QRCodeGeneratorApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // Content for the QR code
            string content = "https://www.example.com";

            // Initialize the QRCodeController with the content
            QRCodeController qrController = new QRCodeController(content);

            // Optional: Set error correction level (default is Q)
            qrController.SetErrorCorrectionLevel(QRCoder.QRCodeGenerator.ECCLevel.H);

            // Optional: Set the size of each module in the QR code (default is 20)
            qrController.SetPixelsPerModule(25);

            // Save the QR code image with logo
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "QRCodeWithLogo.png");
            string logoPath = @"C:\Users\Disable\Pictures\Screenshots\Снимок экрана (1).png";
            qrController.SaveQRCodeWithLogo(filePath, logoPath, logoSize: 50); // Adjust logoSize as needed

            Console.WriteLine("QR Code with logo generated and saved successfully at " + filePath);
            /*            // Content for the QR code
                        string content = "https://www.example.com";

                        // Initialize the QRCodeController with the content
                        QRCodeController qrController = new QRCodeController(content);

                        // Optional: Set error correction level (default is Q)
                        qrController.SetErrorCorrectionLevel(QRCoder.QRCodeGenerator.ECCLevel.H);

                        // Optional: Set the size of each module in the QR code (default is 20)
                        qrController.SetPixelsPerModule(25);

                        // Generate the QR code image
                        Bitmap qrImage = qrController.GetQRCodeImage();

                        // Save the QR code image to the Desktop
                        string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "QRCode.png");
                        qrController.SaveQRCodeToFile(filePath);

                        // Optionally, get the QR code as a byte array for further use
                        byte[] qrCodeBytes = qrController.GetQRCodeAsBytes();

                        Console.WriteLine("QR Code generated and saved successfully at " + filePath);*/
        }
    }
}
