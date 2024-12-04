using Microsoft.EntityFrameworkCore;
using QRCodeGeneratorApp.Models;

namespace QRCodeGeneratorApp.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<QRCodeEntity>? QRCodes { get; set; } // Таблица для хранения QR-кодов
        public DbSet<FileEntity>? Files { get; set; }     // Таблица для файлов

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            ConfigureFileEntity(modelBuilder);
            ConfigureQRCodeEntity(modelBuilder);
        }

        private void ConfigureQRCodeEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<QRCodeEntity>(entity =>
            {
                entity.HasKey(e => e.Id); // Устанавливаем первичный ключ
                entity.Property(e => e.Data).IsRequired(); // Обязательное поле
                entity.Property(e => e.QRImage).IsRequired(); // Обязательное поле
            });
        }

        private void ConfigureFileEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FileEntity>(entity =>
            {
                entity.HasKey(e => e.Id); // Устанавливаем первичный ключ
                entity.Property(e => e.FileName).IsRequired(); // Обязательное поле
                entity.Property(e => e.SavedDate).IsRequired(); // Обязательное поле
                entity.Property(e => e.DeletionDate).IsRequired(); // Обязательное поле
                entity.Property(e => e.Link).IsRequired(); // Обязательное поле
            });
        }
    }
}
