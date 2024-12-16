using Microsoft.AspNetCore.Mvc;
using QRCodeGeneratorApp.Services;
using System.Drawing;
using System.Drawing.Imaging;
namespace MyQRCodeApp.Controllers
{
    [ApiController]
    [Route("api/QRCode")]
    public class QRCodeController : ControllerBase
    {
        private readonly QRCodeService _qrCodeService;
        private readonly LinkGeneratorService _linkGeneratorService; // Добавлен сервис LinkGeneratorService

        // Инъекция зависимостей
        public QRCodeController(QRCodeService qrCodeService, LinkGeneratorService linkGeneratorService)
        {
            _qrCodeService = qrCodeService;
            _linkGeneratorService = linkGeneratorService; // Инициализация
        }

        // Генерация QR-кода и его сохранение
        [HttpPost("generate")]
        public IActionResult GenerateAndSaveQRCode([FromBody] QRCodeRequest request)
        {
            if (string.IsNullOrEmpty(request.Data))
            {
                return BadRequest("The data field is required.");
            }

            // Генерация QR-кода с фиксированными цветами
            var qrImage = _qrCodeService.GenerateQRCode(request.Data);

            // Сохранение QR-кода в базе данных
            _qrCodeService.SaveQRCodeToDatabase(request.Data, qrImage);

            // Отправляем изображение в виде байтов
            using (var ms = new MemoryStream())
            {
                qrImage.Save(ms, ImageFormat.Png); // Сохраняем в формате PNG
                return File(ms.ToArray(), "image/png");
            }
        }

        // Новый метод для генерации ссылки на Wi-Fi
        [HttpPost("generateWiFiLink")]
        public IActionResult GenerateWiFiLink([FromBody] WiFiRequest request)
        {
            // Использование метода LinkGeneratorService для генерации ссылки на Wi-Fi
            var wifiLink = _linkGeneratorService.GenerateWiFiLink(request.Ssid, request.Password, request.EncryptionType);
            return Ok(wifiLink);
        }

        // Новый метод для генерации ссылки на изображение
        [HttpPost("generateImageLink")]
        public async Task<IActionResult> GenerateImageLink(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Файл не загружен");

            // Указываем путь для сохранения файла
            var uploadsPath = Path.Combine("/app/uploads");

            // Убедитесь, что директория существует
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }

            var filePath = Path.Combine(uploadsPath, file.FileName);

            // Сохраняем файл во временное хранилище
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Генерация ссылки
            var imageUrl = _linkGeneratorService.GenerateLinkFromImage(filePath);

            return Ok(imageUrl);
        }



        // Новый метод для загрузки файла в Dropbox
        [HttpPost("uploadFile")]
        public async Task<IActionResult> UploadFile([FromForm] FileRequest request)
        {
            if (request.File == null || request.File.Length == 0)
            {
                return BadRequest(new { message = "Файл не предоставлен или пуст" });
            }

            // Использование метода LinkGeneratorService для загрузки файла в Dropbox
            var dropboxLink = await _linkGeneratorService.UploadFileToDropBox(request.File, request.DaysUntilDeletion);
            if (dropboxLink != null)
            {
                return Ok(new { link = dropboxLink });
            }
            return BadRequest(new { message = "Ошибка загрузки файла в Dropbox" });
        }

        [HttpGet("test")]
        public IActionResult Test()
        {
            Console.WriteLine("Test endpoint was hit");  // Логирование для отладки
            return Ok("Сервер работает!");
        }

        [HttpGet("history")]
        public IActionResult GetQRCodeHistory([FromQuery] int lastLoadedId, [FromQuery] int take)
        {
            try
            {
                Console.WriteLine($"Получен запрос: lastLoadedId = {lastLoadedId}, take = {take}");

                var qrCodes = _qrCodeService.GetQRCodeHistory(lastLoadedId, take);

                if (qrCodes == null || !qrCodes.Any())
                {
                    return NotFound(new { message = "История QR-кодов пуста." });
                }

                Console.WriteLine($"Возвращаю записи: {string.Join(", ", qrCodes.Select(q => q.Id))}");
                return Ok(qrCodes);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении истории QR-кодов: {ex.Message}");
                return StatusCode(500, new { message = "Произошла ошибка при получении данных." });
            }
        }


    }

    // Запросы для получения данных из тела запроса

    public class QRCodeRequest
    {
        public required string Data { get; set; }
    }

    public class WiFiRequest
    {
        public required string Ssid { get; set; }
        public required string Password { get; set; }
        public required string EncryptionType { get; set; }
    }

    public class ImageRequest
    {
        public required string FilePath { get; set; }
    }

    public class FileRequest
    {
        public required IFormFile File { get; set; }
        public required int DaysUntilDeletion { get; set; }
    }
}
