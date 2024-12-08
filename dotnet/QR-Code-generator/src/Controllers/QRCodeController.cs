using Microsoft.AspNetCore.Mvc;
using QRCodeGeneratorApp.Services;
using System.Drawing;

namespace MyQRCodeApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
        public IActionResult GenerateAndSaveQRCode([FromBody] QRCodeRequest request, Color color)
        {
            // Генерация QR-кода
            var qrBitmap = _qrCodeService.GenerateQRCode(request.Data, color, Color.White);

            // Сохранение QR-кода в базу данных
            _qrCodeService.SaveQRCodeToDatabase(request.Data, qrBitmap);

            return Ok(new { message = "QR-код успешно сгенерирован и сохранен!" });
        }

        // Новый метод для генерации ссылки на Wi-Fi
        [HttpPost("generateWiFiLink")]
        public IActionResult GenerateWiFiLink([FromBody] WiFiRequest request)
        {
            // Использование метода LinkGeneratorService для генерации ссылки на Wi-Fi
            var wifiLink = _linkGeneratorService.GenerateWiFiLink(request.Ssid, request.Password, request.EncryptionType);
            return Ok(new { link = wifiLink });
        }

        // Новый метод для генерации ссылки на изображение
        [HttpPost("generateImageLink")]
        public async Task<IActionResult> GenerateImageLink([FromBody] ImageRequest request)
        {
            // Использование метода LinkGeneratorService для получения ссылки на изображение
            var imageUrl = _linkGeneratorService.GenerateLinkFromImage(request.FilePath);
            return Ok(new { link = imageUrl });
        }

        // Новый метод для загрузки файла в Dropbox
        [HttpPost("uploadFile")]
        public async Task<IActionResult> UploadFile([FromBody] FileRequest request)
        {
            // Использование метода LinkGeneratorService для загрузки файла в Dropbox
            var dropboxLink = await _linkGeneratorService.UploadFileToDropBox(request.FileName, request.DaysUntilDeletion);
            if (dropboxLink != null)
            {
                return Ok(new { link = dropboxLink });
            }
            return BadRequest(new { message = "Ошибка загрузки файла в Dropbox" });
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
        public required string FileName { get; set; }
        public required int DaysUntilDeletion { get; set; }
    }
}
