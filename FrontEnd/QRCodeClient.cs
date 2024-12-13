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

        }


        // Генерация QR-кода
        public async Task<QRCodeGenerationResponse> GenerateQRCodeAsync(string data)
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
                    // Возвращаем ошибку, если статус не успешен
                    return new QRCodeGenerationResponse
                    {
                        Message = $"Ошибка: {response.StatusCode}, {await response.Content.ReadAsStringAsync()}"
                    };
                }
                else
                {
                    // Чтение байтов изображения из ответа
                    var imageBytes = await response.Content.ReadAsByteArrayAsync();
                    return new QRCodeGenerationResponse
                    {
                        ImageBytes = imageBytes,
                        Message = "QR-код успешно сгенерирован!"
                    };
                }

            }
            catch (HttpRequestException httpEx)
            {
                return new QRCodeGenerationResponse
                {
                    Message = $"Ошибка запроса: {httpEx.Message}"
                };
            }
            catch (Exception ex)
            {
                return new QRCodeGenerationResponse
                {
                    Message = $"Ошибка: {ex.Message}"
                };
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
                string result = await response.Content.ReadAsStringAsync();
                return result;
            }
            else
            {
                Console.WriteLine("Ошибка при генерации Wi-Fi ссылки!");
                return null;
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
    public class QRCodeGenerationResponse
    {
        public byte[] ImageBytes { get; set; }
        public string Message { get; set; }
    }

}
