using System;
using System.ComponentModel.DataAnnotations;

namespace QRCodeGeneratorApp.Models
{
    public class QRCodeEntity
    {
        [Key]        
        public int Id { get; set; } // Уникальный идентификатор

        public required string Data { get; set; } // Содержимое QR-кода
        
        public required byte[] QRImage { get; set; } // Изображение QR-кода как массив байт

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
