using System;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Newtonsoft.Json;
using Serilog;
using Xceed.Wpf.Toolkit;

namespace FrontEnd
{
    public class QRCodeClient
    {
        private readonly HttpClient _httpClient;

        // Конструктор с передачей HttpClient
        public QRCodeClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }


        // Генерация QR-кода
        public async Task<Bitmap> GenerateQRCodeAsync(string data)
        {
            var qrCodeRequest = new
            {
                Data = data
            };

            var jsonContent = JsonConvert.SerializeObject(qrCodeRequest);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync("generate", content);

                // Проверяем статус ответа
                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                // Чтение байтов изображения из ответа
                var imageBytes = await response.Content.ReadAsByteArrayAsync();

                // Если байты пустые, это может указывать на проблему с ответом
                if (imageBytes == null || imageBytes.Length == 0)
                {
                    Console.WriteLine("Received empty byte array for the QR code image.");
                    return null;
                }

                // Преобразуем байты изображения в Bitmap
                using (var ms = new MemoryStream(imageBytes))
                {
                    try
                    {
                        var bitmap = new System.Drawing.Bitmap(ms);
                        return bitmap;
                    }
                    catch (Exception ex)
                    {
                        // Логируем ошибку при создании Bitmap
                        Console.WriteLine($"Ошибка при создании Bitmap: {ex.Message}");
                        return null;
                    }
                }
            }
            catch (HttpRequestException httpEx)
            {
                // Логируем ошибку HTTP-запроса
                Console.WriteLine($"Ошибка HTTP-запроса: {httpEx.Message}");
                return null;
            }
            catch (Exception ex)
            {
                // Логируем любые другие ошибки
                Console.WriteLine($"Ошибка: {ex.Message}");
                return null;
            }
        }



        // Генерация ссылки на Wi-Fi
        public async Task<string> GenerateWiFiLinkAsync(string ssid, string password, string encryptionType)
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
                return result;
            }

            Console.WriteLine("Ошибка при генерации Wi-Fi ссылки!");
            return null;
        }

        // Генерация ссылки на изображение
        public async Task<string> GenerateImageLinkAsync(string filePath)
        {
            using (var form = new MultipartFormDataContent())
            {
                // Читаем файл в поток
                using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    var fileContent = new StreamContent(fileStream);
                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

                    // Добавляем файл в форму
                    form.Add(fileContent, "file", Path.GetFileName(filePath));

                    // Отправляем POST-запрос
                    var response = await _httpClient.PostAsync("generateImageLink", form);

                    // Читаем ответ
                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"Ссылка на изображение: {result}");
                        return result;
                    }
                    else
                    {
                        Console.WriteLine("Ошибка при генерации ссылки на изображение!");
                        return null;
                    }
                }
            }
        }



        // Загрузка файла в Dropbox
        public async Task<string> UploadFileAsync(string filePath, int daysUntilDeletion)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine("Указанный файл не существует!");
                return "nonexist";
            }

            using (var form = new MultipartFormDataContent())
            {
                using (var fileStream = File.OpenRead(filePath))
                {
                    var fileContent = new StreamContent(fileStream);
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                    // Добавление файла в запрос
                    form.Add(fileContent, "file", Path.GetFileName(filePath));

                    // Добавление дополнительных данных в запрос
                    form.Add(new StringContent(daysUntilDeletion.ToString()), "daysUntilDeletion");

                    // Отправка запроса
                    var response = await _httpClient.PostAsync("uploadFile", form);

                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"Файл загружен в Dropbox: {result}");
                        return result;
                    }

                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Ошибка при загрузке файла в Dropbox: {response.StatusCode} - {error}");
                    return response.StatusCode.ToString();
                }
            }
        }
    }

    public class QRCodeGenerationResponse
    {
        public byte[] ImageBytes { get; set; }
        public string Message { get; set; }
    }
}