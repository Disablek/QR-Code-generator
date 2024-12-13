using System;
using System.Drawing;
using System.Threading.Tasks;
using QRCodeGeneratorApp.Data;
using QRCodeGeneratorApp.Models;
using QRCodeGeneratorApp.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;


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

        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddControllers();  // Добавляем поддержку контроллеров

        // Регистрация сервисов
        builder.Services.AddScoped<QRCodeService>();
        builder.Services.AddScoped<LinkGeneratorService>();

        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();
        builder.Logging.SetMinimumLevel(LogLevel.Debug);

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });

        builder.WebHost.UseUrls("http://172.17.0.2:5000"); // Использование локального IP

        var app = builder.Build();
        app.Use(async (context, next) =>
        {
            try
            {
                await next.Invoke();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                await context.Response.WriteAsync("Произошла ошибка на сервере");
            }
        });

        app.UseRouting();
        app.UseCors("AllowAll");
        app.MapControllers();
        app.MapFallback(() => Results.NotFound("Маршрут не найден"));

        // Простой endpoint для проверки состояния
        app.MapGet("/health", () =>
        {
            Console.WriteLine("Health check received");
            return Results.Ok("Server is running");
        });
        app.MapGet("/", () => "Hello World!");

        Console.WriteLine("Server is starting...");
        
        // Запуск логирования каждую секунду в фоновом режиме
        _ = Task.Run(() => StartLogging());
        try
        {
            Console.WriteLine("Приложение запускается...");
            await app.RunAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при запуске приложения: {ex.Message}");
        }

    }

    public static async Task StartLogging()
    {
        while (true)
        {
            Console.WriteLine($"[{DateTime.Now}] Сервер работает...");  // Логирование каждую секунду
            await Task.Delay(10000); // Задержка в 1 секунду
        }
    }
}
