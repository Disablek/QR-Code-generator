using System;
using System.Drawing;
/*using QRCodeGeneratorApp.Controller;*/
using QRCodeGeneratorApp.Models;

class Program
{
    static void Main()
    {
        QRCodeModel QR = new QRCodeModel("example.com");
        QR.XUI(@"C:\Users\Disable\Desktop\QRCodeWithLogo.png");

    }
}
