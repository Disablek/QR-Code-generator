using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace FrontEnd.Pages
{
    public partial class HistoryPage : Page
    {
        private const string ApiUrl = "http://localhost:5000/api/QRCode/history"; // URL API
        private const int take = 20; // Количество записей за один запрос
        private readonly MainWindow _mainWindow; // Ссылка на MainWindow
        private bool isLoading; // Флаг загрузки
        private int lastLoadedId; // Сохраняем последний загруженный Id

        public HistoryPage(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow; // Сохраняем ссылку на MainWindow
        }

        public async Task LoadQRCodeHistory()
        {
            if (isLoading) return; // Предотвращение одновременных запросов
            isLoading = true;

            try
            {
                _mainWindow.ShowWaitingAnimationAsync(true, false);

                // Логируем текущий lastLoadedId перед запросом
                Console.WriteLine($"[Клиент] Отправляю запрос с lastLoadedId = {lastLoadedId}");

                // Получение данных с сервера
                var historyItems = await FetchQRCodeHistory(lastLoadedId, take);

                if (historyItems == null || historyItems.Count == 0)
                {
                    Console.WriteLine("[Клиент] Больше данных нет.");
                    return; // Нет больше данных для загрузки
                }

                foreach (var item in historyItems)
                    // Убедимся, что элементы не дублируются
                    if (!HistoryDataGrid.Items.Cast<QRCodeHistoryItem>().Any(i => i.Id == item.Id))
                        HistoryDataGrid.Items.Add(item);

                // Обновляем lastLoadedId до минимального значения Id
                lastLoadedId = historyItems.Min(item => item.Id);

                // Логируем обновленный lastLoadedId
                Console.WriteLine($"[Клиент] Обновленный lastLoadedId = {lastLoadedId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Клиент] Ошибка: {ex.Message}");
            }
            finally
            {
                isLoading = false;
                _mainWindow.ShowWaitingAnimationAsync(false, false);
            }
        }


        private async Task<ObservableCollection<QRCodeHistoryItem>> FetchQRCodeHistory(int lastLoadedId, int take)
        {
            var client = new HttpClient();
            var response = await client.GetAsync($"{ApiUrl}?lastLoadedId={lastLoadedId}&take={take}");

            if (!response.IsSuccessStatusCode) throw new Exception($"Ошибка запроса: {response.StatusCode}");

            var responseContent = await response.Content.ReadAsStringAsync();
            var items = JsonSerializer.Deserialize<QRCodeHistoryItem[]>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var historyItems = new ObservableCollection<QRCodeHistoryItem>();

            foreach (var item in items)
            {
                item.QRImage = await ConvertBase64ToBitmapImage(item.QRImageBase64); // Преобразуем Base64 в изображение
                historyItems.Add(item);
            }

            return historyItems;
        }


        private async void HistoryDataGrid_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var verticalOffset = e.VerticalOffset;
            var scrollableHeight = e.ExtentHeight - e.ViewportHeight;

            // Если пользователь прокрутил до конца
            if (verticalOffset == scrollableHeight && !isLoading) await LoadQRCodeHistory(); // Загружаем больше данных
        }

        // Конвертация Base64 в BitmapImage
        private async Task<BitmapImage> ConvertBase64ToBitmapImage(string base64)
        {
            var binaryData = Convert.FromBase64String(base64);
            var ms = new MemoryStream(binaryData);
            var image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.StreamSource = ms;
            image.EndInit();
            return image;
        }
    }
}

// DTO-класс для представления QR-кодов
public class QRCodeHistoryItem
{
    public int Id { get; set; }
    public string Data { get; set; }
    public DateTime CreatedAt { get; set; }
    public string QRImageBase64 { get; set; } // Изображение в формате Base64
    public BitmapImage QRImage { get; set; } // Преобразованное изображение для отображения
}