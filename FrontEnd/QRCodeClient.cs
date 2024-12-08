using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FrontEnd
{
    public class QRCodeClient
    {
        private readonly HttpClient _httpClient;

        // Конструктор с передачей HttpClient
        public QRCodeClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:5000/api/QRCode/"); // Адрес вашего API
        }

        // Генерация QR-кода
        public async Task GenerateQRCodeAsync(string data)
        {
            var qrCodeRequest = new
            {
                Data = data
            };

            var jsonContent = JsonConvert.SerializeObject(qrCodeRequest);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("generate", content);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("QR код успешно создан!");
            }
            else
            {
                Console.WriteLine("Ошибка при создании QR кода!");
            }
        }

        // Генерация ссылки на Wi-Fi
        public async Task GenerateWiFiLinkAsync(string ssid, string password, string encryptionType)
        {
            var wifiRequest = new
            {
                Ssid = ssid,
                Password = password,
                EncryptionType = encryptionType
            };

            var jsonContent = JsonConvert.SerializeObject(wifiRequest);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("generateWiFiLink", content);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Wi-Fi ссылка: {result}");
            }
            else
            {
                Console.WriteLine("Ошибка при генерации Wi-Fi ссылки!");
            }
        }

        // Генерация ссылки на изображение
        public async Task GenerateImageLinkAsync(string filePath)
        {
            var imageRequest = new
            {
                FilePath = filePath
            };

            var jsonContent = JsonConvert.SerializeObject(imageRequest);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("generateImageLink", content);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Ссылка на изображение: {result}");
            }
            else
            {
                Console.WriteLine("Ошибка при генерации ссылки на изображение!");
            }
        }

        // Загрузка файла в Dropbox
        public async Task UploadFileAsync(string fileName, int daysUntilDeletion)
        {
            var fileRequest = new
            {
                FileName = fileName,
                DaysUntilDeletion = daysUntilDeletion
            };

            var jsonContent = JsonConvert.SerializeObject(fileRequest);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("uploadFile", content);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Файл загружен в Dropbox: {result}");
            }
            else
            {
                Console.WriteLine("Ошибка при загрузке файла в Dropbox!");
            }
        }
    }
}
