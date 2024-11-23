using System;
using System.Drawing;
/*using QRCodeGeneratorApp.Controller;*/
using System.Threading.Tasks;
using QRCodeGeneratorApp.Models;
using Python.Runtime;
using Dropbox.Api;
using Dropbox.Api.Files;

class Program
{
    static async Task Main(string[] args)
    {
        // Генерация QR-кода
        QRCodeModel QR = new QRCodeModel("https://i.ibb.co/Tm8qRm9/niggers.webp");
        QR.CreateQRCodeWithLogo(@"/host_desktop/Desktop/QRCodeWithLogo.png");
        QR.CreateQRCode(@"/host_desktop/Desktop/QRCode.png");

        GenerateLinkFromImage(@"/host_desktop/Desktop/niggers.jpg");
        // Загрузка файла и получение ссылки
        string dropboxLink = await UploadFileToDropBox();
        Console.WriteLine($"Ссылка на загруженный файл: {dropboxLink}");
    }

    static string GenerateLinkFromImage(string filepath)
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

    static async Task<string> UploadFileToDropBox()
    {
        // Ваш Access Token (замените на ваш)
        string accessToken = "sl.u.AFXgexDRQYgvMbPIjHiB31SpyrwaLRL8I1QFmRW_xipjsiHEvYvYCfV0v56uZK_gwX3pZuFXpfdRWA1LzbjGBtC5rJcCHeqYBCy6__XX6Xh-_9hAIA3FwdArIuOYNm52DB6Z9fWxHc-koM41-yqIJdFI6kwHAkii43R-wwKkKBXhKgptcCtCNLhQ1N8pAMPDpeaSoUzMZQsP61vHzs2lc_wLIxj75BQIRPYgewbP1s2nljdBDAWy35LNNVvuSdQPV44vHAnB4nsW3h-giyQebnYyYVGWyLjvjcawdUCFgWo8inzwyQ_dL44UHnV1TnNB7Jj6mXexlvhqRZ9WrXZ__vu3uAd_ECafPdA0zOQhfYN4jpa09JMYhASxL7GHOfo5AytcvQDiqdidEMGWGJIzzaklOtqNwoTzQwK_pygW2b2rw5bawbtl3CW-g-u3Sta4r5lLaW7jZyIyykVTG7qXVt39BqD9x6gdTEPkBRq5m606qHd5E-LGT_cV2keO1oJk3nLHKvLWw4F22CB9OWYxvmerpX_SL2kKjIdZFgXDcTyerqlgQtxJ7z6Wv7wcRfRfRyUo_tYJbH2ttMilH0iH-3zUbc0KsTW25QdwC_0u9xjdLMJy6WgQIFjWiV5dG71OAMAAR4fytvjLK4CODPQyD60ugV9xEU7Yo1NbHYSDjkyERqClFRmA2FI1_7P5SUP9RlxAhC1LBrOhEbUycMOqltq9VIad3MbhxxKOYUQ1K08LnhD3oyGz5spWYEwgjJpyM3jSQCB2r3vKxWcdOlK2mBpd3nIXLh-nFF-gzb7ZnAKl8B0kPp5D6Kw0Ifhd_HuarXJEdBIFAlJf9OGR5VH63N5xZSSac_6K8n4260S_KDBjSEexBROzJWIkyvFldp5XzUincv2EtpPayMz44MePKFMdpwLW8M9SYuiU4c3pflL6i5NUCcmePy_ASWAatfTjhUs5my0mR09ezA4ya8HDXDHoLTBK16ofNt9Jj-uYMA58u920mUaD9qFk7r-AL5q4i3OK4DH7H7hECbgHLaL6uNpgO1BvS0mvaxBzRo0jDgo4g4ozALf9_ItCqe-2UPUOk5Hg73fHsZcPre53Fp0xyCy0SW5PfKbemR5FXZY9Nsw6jLO3cfN3wkP1xicMtOBDyrD0h4PpoWBwls_m7kOYwRvbWLuhxEjmho4-D-6VISt__HPIoR9zM-OEQqZ4LSg3-gmXkx6vu-8k9j88E7QLSW3o61xHjo6SDspCcKr9oKfKJk5rnSa2vryuizYdauXLpW4xyeyrrhbPDIM14GnodLbX";

        // Путь к локальному файлу, который вы хотите загрузить
        string localFilePath = @"/host_desktop/Desktop/file_example_MP3_1MG.mp3";

        // Путь в Dropbox, куда будет загружен файл
        string dropboxFilePath = "/example.mp3";

        try
        {
            // Инициализация Dropbox клиента
            using (var dbx = new DropboxClient(accessToken))
            {
                // Чтение файла
                using (var fileStream = File.OpenRead(localFilePath))
                {
                    // Загрузка файла в Dropbox
                    var uploadResponse = await dbx.Files.UploadAsync(
                        path: dropboxFilePath,
                        mode: WriteMode.Overwrite.Instance,
                        body: fileStream);

                    Console.WriteLine($"Файл загружен в Dropbox: {uploadResponse.PathDisplay}");
                }

                // Получение общей ссылки на файл
                var sharedLinkResponse = await dbx.Sharing.CreateSharedLinkWithSettingsAsync(dropboxFilePath);
                Console.WriteLine($"Ссылка на файл: {sharedLinkResponse.Url}");
                return sharedLinkResponse.Url;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
        return null;
    }
}
