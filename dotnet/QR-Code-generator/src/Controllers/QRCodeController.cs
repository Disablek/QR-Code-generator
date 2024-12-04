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

        public QRCodeController(QRCodeService qrCodeService)
        {
            _qrCodeService = qrCodeService;
        }

        [HttpPost("generate")]
        public IActionResult GenerateAndSaveQRCode([FromBody] QRCodeRequest request, Color color)
        {
            // Генерация QR-кода
            var qrBitmap = _qrCodeService.GenerateQRCode(request.Data, color, Color.White);

            // Сохранение QR-кода в базу данных
            _qrCodeService.SaveQRCodeToDatabase(request.Data, qrBitmap);

            return Ok(new { message = "QR-код успешно сгенерирован и сохранен!" });
        }
    }

    public class QRCodeRequest
    {
        public required string Data { get; set; }
    }
}
