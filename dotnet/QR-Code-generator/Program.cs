using System;
using System.Drawing;
using System.Threading.Tasks;
using QRCodeGeneratorApp.Data;
using QRCodeGeneratorApp.Models;
using QRCodeGeneratorApp.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using Microsoft.CodeAnalysis.CSharp.Syntax;

class Program
{   
    static async Task Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        // Настройка базы данных
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseSqlite("Data Source=QRCodesDatabase.db");
        
        using (var connection = new SqliteConnection("Data Source=QRCodesDatabase.db"))
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='Files';";
            var result = command.ExecuteScalar();

            if (result == null)
            {
                Console.WriteLine("Таблица 'Files' не существует.");
            }
            else
            {
                Console.WriteLine("Таблица 'Files' найдена.");
            }
        }

        using var dbContext = new ApplicationDbContext(optionsBuilder.Options);
        
        dbContext.Database.EnsureCreated();

        var qrCodeService = new QRCodeService(dbContext);
        var linkGeneratorService = new LinkGeneratorService(dbContext);

        // //1. Создание текстового QR-кода
        // Console.WriteLine("Создание текстового QR-кода...");
        // string textData = "https://example.com";
        // var textQr = qrCodeService.GenerateQRCode(textData, Color.Black, Color.White);
        // qrCodeService.SaveQRCodeToDatabase(textData, textQr);

        // // 2. Создание WiFi QR-кода
        // Console.WriteLine("Создание WiFi QR-кода...");
        // string wifiSSID = "MyWiFiNetwork";
        // string wifiPassword = "MySecurePassword";
        // string encryptionType = "WPA";
        // string wifiData = linkGeneratorService.GenerateWiFiLink(wifiSSID, wifiPassword, encryptionType);
        // var wifiQr = qrCodeService.GenerateQRCode(wifiData, Color.Black, Color.White);
        // qrCodeService.SaveQRCodeToDatabase(wifiData, wifiQr);

        // 3. Создание QR-кода для файла
        // Console.WriteLine("Создание QR-кода для файла...");
        // string fileName = "file_example_MP3_1MG.mp3";
        // int retentionDays = 1;
        // string fileLink = await linkGeneratorService.UploadFileToDropBox(fileName, retentionDays);
        // if (string.IsNullOrWhiteSpace(fileLink))
        // {
        //     throw new Exception("Ссылка на файл пуста. Проверьте метод UploadFileToDropBox.");
        // }

        
        // var fileQr = qrCodeService.GenerateQRCode(fileLink, Color.Black, Color.White);
        // qrCodeService.SaveQRCodeToDatabase(fileLink, fileQr);

        // // 4. Создание QR-кода с фото
        // Console.WriteLine("Создание QR-кода с фото...");
        // string imagePath = "/host_desktop/Desktop/sample_image.jpg";
        // string imageLink = linkGeneratorService.GenerateLinkFromImage(imagePath);
        // var imageQr = qrCodeService.GenerateQRCode(imageLink, Color.Black, Color.White);
        // qrCodeService.SaveQRCodeToDatabase(imageLink, imageQr);

        // Console.WriteLine("Все QR-коды успешно созданы и сохранены в базе данных!");
    }
}
