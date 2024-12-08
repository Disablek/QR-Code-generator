using System;
using System.Drawing;
/*using QRCodeGeneratorApp.Controller;*/
using QRCodeGeneratorApp.Models;
using QRCodeGeneratorApp.Data;
using Python.Runtime;
using Dropbox.Api;
using Dropbox.Api.Files;
using Microsoft.Data.Sqlite;

namespace QRCodeGeneratorApp.Services{
    public class LinkGeneratorService
    {
        private readonly ApplicationDbContext _context;
        private const string DbPath = "FilesDatabase.db";

        public LinkGeneratorService(ApplicationDbContext context)
        {
            _context = context;
        }

        public string GenerateWiFiLink(string ssid, string password, string encryptionType)
        {
            // Формируем строку для QR-кода Wi-Fi
            string wifiData = $"WIFI:T:{encryptionType};S:{ssid};P:{password};;";
            return wifiData;
        }

        public string GenerateLinkFromImage(string filepath)
        {
            string imageUrl = string.Empty;

            Runtime.PythonDLL = "/usr/lib/x86_64-linux-gnu/libpython3.11.so.1.0";
            PythonEngine.Initialize();

            using (Py.GIL())
            {
                // Добавляем путь к директории с lok.py в sys.path
                dynamic sys = Py.Import("sys");
                sys.path.append(@"/host_desktop/GitHub/Cursed/QR-Code-generator/python/");

                // Импортируем скрипт lok и вызываем функцию createimgBB
                var pythonscript = Py.Import("lok");
                var message = new PyString(filepath);
                var result = pythonscript.InvokeMethod("createimgBB", new PyObject[] { message });
                imageUrl = result.ToString();
            }

            Console.WriteLine($"URL загруженного изображения: {imageUrl}");
            return imageUrl;
        }

        /*
        Создание Таблицы Files для отслеживания загрузки файлов в DropBox
        */
        public async Task<string> UploadFileToDropBox(string fileName, int daysUntilDeletion)
        {
            string accessToken = Environment.GetEnvironmentVariable("DROPBOX_ACCESS_TOKEN");

            if (string.IsNullOrEmpty(accessToken))
            {
                Console.WriteLine("Переменная DROPBOX_ACCESS_TOKEN не найдена!");
                return null;
            }

            string localFilePath = @$"C:\Users\Disable\Desktop\{fileName}";
            string dropboxFilePath = $"/{fileName}";

            try
            {
                using var dbx = new DropboxClient(accessToken);
                await RemoveExpiredFiles(dbx);

                // Проверка существования файла
                try
                {
                    var metadata = await dbx.Files.GetMetadataAsync(dropboxFilePath);
                    Console.WriteLine("Файл с таким именем уже существует. Переименовываем файл.");

                    // Генерация нового имени для файла
                    string newFileName = $"{Path.GetFileNameWithoutExtension(fileName)}_{Guid.NewGuid()}{Path.GetExtension(fileName)}";
                    dropboxFilePath = $"/{newFileName}";
                    Console.WriteLine($"Переименованный файл: {dropboxFilePath}");
                }
                catch (Dropbox.Api.ApiException<Dropbox.Api.Files.GetMetadataError> ex)
                {
                    if (ex.ErrorResponse.IsPath)
                    {
                        // Переименовать файл, если он существует
                        string newFileName = $"{Path.GetFileNameWithoutExtension(fileName)}_{Guid.NewGuid()}{Path.GetExtension(fileName)}";
                        dropboxFilePath = $"/{newFileName}";
                        Console.WriteLine($"Переименованный файл: {dropboxFilePath}");
                    }
                    else
                    {
                        Console.WriteLine("Ошибка при проверке существующего файла: " + ex.Message);
                    }
                }

                // Загрузка файла
                using var fileStream = File.OpenRead(localFilePath);
                var uploadResponse = await dbx.Files.UploadAsync(
                    path: dropboxFilePath,
                    mode: WriteMode.Overwrite.Instance,
                    body: fileStream);

                var sharedLinkResponse = await dbx.Sharing.CreateSharedLinkWithSettingsAsync(dropboxFilePath);
                string dropboxLink = sharedLinkResponse.Url;

                Console.WriteLine($"Файл загружен: {uploadResponse.PathDisplay}");

                // Сохранение информации в БД
                await SaveFileInfoToDatabase(fileName, DateTime.Now, DateTime.Now.AddMinutes(daysUntilDeletion * 5), dropboxLink);

                return dropboxLink;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                return null;
            }
        }



        static async Task RemoveExpiredFiles(DropboxClient dbx)
        {
            const string DbPath = "QRCodesDatabase.db";

            // Убедимся, что таблица существует
            EnsureFilesTableExists();
            
            using var connection = new Microsoft.Data.Sqlite.SqliteConnection($"Data Source={DbPath}");
            connection.Open();

            // Запрос для выборки записей с истёкшим временем удаления
            string selectQuery = @"
                SELECT *
                FROM Files 
                WHERE DeletionDate <= @CurrentDate";

            using var selectCommand = new Microsoft.Data.Sqlite.SqliteCommand(selectQuery, connection);
            selectCommand.Parameters.AddWithValue("@CurrentDate", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

            // Выполнение запроса
            using var reader = selectCommand.ExecuteReader();

            if (!reader.HasRows)
            {
                Console.WriteLine("Нет записей для удаления.");
                return;
            }

            while (reader.Read())
            {
                string fileName = reader["FileName"].ToString();
                string fileLink = reader["Link"].ToString();

                try
                {
                    // Удаление файла из Dropbox
                    string dropboxFilePath = $"/{fileName}";
                    try
                    {
                        var metadata = await dbx.Files.GetMetadataAsync(dropboxFilePath);
                        // Если файл найден, удаляем его
                        await dbx.Files.DeleteV2Async(dropboxFilePath);
                        Console.WriteLine($"Файл {fileName} удалён из Dropbox.");
                    }
                    catch (Dropbox.Api.ApiException<Dropbox.Api.Files.GetMetadataError> ex)
                    {
                        if (ex.ErrorResponse.IsPath && ex.ErrorResponse.AsPath.Value.IsNotFound)
                        {
                            Console.WriteLine($"Файл {fileName} не найден в Dropbox, пропускаем удаление.");
                        }
                        else
                        {
                            Console.WriteLine($"Ошибка удаления {fileName} из Dropbox: {ex.Message}");
                        }
                    }

                    Console.WriteLine($"Файл {fileName} удалён из Dropbox.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка удаления {fileName} из Dropbox: {ex.Message}");
                }
            }

            // Удаление записей из базы данных
            string deleteQuery = @"
                DELETE FROM Files 
                WHERE datetime(DeletionDate) <= datetime(@CurrentDate)";
            
            using var deleteCommand = new Microsoft.Data.Sqlite.SqliteCommand(deleteQuery, connection);
            deleteCommand.Parameters.AddWithValue("@CurrentDate", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

            int deletedRows = deleteCommand.ExecuteNonQuery();
            Console.WriteLine($"Удалено записей из базы данных: {deletedRows}");
        }


        // Убедимся, что таблица Files существует
        static void EnsureFilesTableExists()
        {
            const string DbPath = "FilesDatabase.db";

            using var connection = new Microsoft.Data.Sqlite.SqliteConnection($"Data Source={DbPath}");
            connection.Open();

            string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS Files (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    FileName TEXT NOT NULL,
                    SavedDate DATETIME NOT NULL,
                    DeletionDate DATETIME NOT NULL,
                    Link TEXT NOT NULL
                );";

            using var createCommand = new Microsoft.Data.Sqlite.SqliteCommand(createTableQuery, connection);
            createCommand.ExecuteNonQuery();

            Console.WriteLine("Таблица 'Files' проверена или создана.");
        }


        private async Task SaveFileInfoToDatabase(string fileName, DateTime savedDate, DateTime deletionDate, string link)
        {
            var fileEntity = new FileEntity
            {
                FileName = fileName,
                SavedDate = savedDate,
                DeletionDate = deletionDate,
                Link = link
            };

            _context.Files.Add(fileEntity);
            await _context.SaveChangesAsync();
        }       
    }
}