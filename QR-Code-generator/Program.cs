using System;
using System.Drawing;
/*using QRCodeGeneratorApp.Controller;*/
using QRCodeGeneratorApp.Models;

class Program
{
    static void Main()
    {
        QRCodeModel QR = new QRCodeModel("https://i.ibb.co/Tm8qRm9/niggers.webp");
        QR.CreateQRCodeWithLogo(@"C:\Users\Disable\Desktop\QRCodeWithLogo.png");
        QR.CreateQRCode(@"C:\\Users\\Disable\\Desktop\\QRCodeWithLogo.png");
        Console.WriteLine(QR.GenerateLinkFromImage((@"C:\Users\Disable\Desktop\niggers.jpg")));
    }
}
