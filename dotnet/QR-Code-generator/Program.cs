using System;
using System.Drawing;
/*using QRCodeGeneratorApp.Controller;*/
using QRCodeGeneratorApp.Models;
using Python.Runtime;

class Program
{
    static void Main()
    {
        QRCodeModel QR = new QRCodeModel("https://i.ibb.co/Tm8qRm9/niggers.webp");
        Console.WriteLine('0');
        QR.CreateQRCodeWithLogo(@"/host_desktop/Desktop/QRCodeWithLogo.png");
        QR.CreateQRCode(@"/host_desktop/Desktop/QRCode.png");
        Console.WriteLine('1');
        string xui = null;
        Console.WriteLine(xui != null);
        xui = GenerateLinkFromImage(@"/host_desktop/Desktop/niggers.jpg");
        Console.WriteLine(xui);
    }
    static public string GenerateLinkFromImage(string filepath)
        {
            Console.WriteLine('2');
            string imageUrl = string.Empty;

            Runtime.PythonDLL = @"/host_desktop/AppData/Local/Programs/Python/Python311/python311.dll";
            PythonEngine.Initialize();

            using (Py.GIL())
            {
                // Добавляем путь к директории с lok.py в sys.path
                dynamic sys = Py.Import("sys");
                sys.path.append(@"/host_desktop/Source/Repos/QR-Code-generator/QR-Code-generator/src/Models");
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
}
