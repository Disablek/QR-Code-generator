using System;
using System.Windows;
using System.Net.Http;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit;
using System.Windows.Controls;

namespace FrontEnd
{
    public partial class MainWindow : Window
    {
        private string _currentLanguage = "ru";
        private HttpClient _httpClient; // Убираем создание нового клиента здесь

        public MainWindow()
        {
            InitializeComponent();
            _httpClient = new HttpClient(); // Инициализируем один клиент
        }

        private void ButtonQRText_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new TextPage());
        }

        private void ButtonQRWiFi_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new WiFiPage());
        }

        private void ButtonQRFile_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new FilePage());
        }

        private void ButtonQRURL_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new LinkPage());
        }

        private void ButtonQRPhoto_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new PhotoPage());
        }

        private void ButtonGenerateQR_Click(object sender, RoutedEventArgs e)
        {
            // Получаем ссылку на активную страницу
            Page activePage = ContentFrame.Content as Page;
            string QRERROR = Application.Current.Resources["QRCodeGenerateError"] as String;
            if (string.IsNullOrEmpty(QRERROR))
            {
                QRERROR = "Сообщение об ошибке не найдено"; // Резервное сообщение
            }

            if (activePage == null)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show($"{QRERROR}", "Ошибка!");
                return;
            }
            string pageName = activePage.GetType().Name;
            if (pageName == "TextPage" || pageName == "LinkPage")
            {
                // Ищем элемент на активной странице по имени
                TextBox myTextBox = activePage.FindName("myTextBox") as TextBox;
            }
        }

        // Пример асинхронного метода для обращения к API
        private async Task CallApiAsync(string url)
        {
            try
            {
                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    // Обработка полученного контента
                }
                else
                {
                    // Обработка ошибки API
                    Xceed.Wpf.Toolkit.MessageBox.Show("Ошибка при обращении к API");
                }
            }
            catch (Exception ex)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void ChangeDictianory(object sender, RoutedEventArgs e)
        {
            // Очистить текущие словари ресурсов
            Resources.MergedDictionaries.Clear();

            // Определить путь к новому словарю
            string newDictionaryPath = _currentLanguage == "ru"
                ? "Dictianory/Resources.en.xaml"
                : "Dictianory/Resources.ru.xaml";

            // Загрузить новый словарь
            var newDictionary = new ResourceDictionary
            {
                Source = new Uri(newDictionaryPath, UriKind.Relative)
            };

            // Добавить новый словарь в ресурсы
            Resources.MergedDictionaries.Add(newDictionary);

            // Сменить текущий язык
            _currentLanguage = _currentLanguage == "ru" ? "en" : "ru";
        }

        // Пример вызова метода генерации QR-кода
        private async void GenerateQRCode(string data)
        {
            QRCodeClient qrCodeClient = new QRCodeClient(_httpClient);
            await qrCodeClient.GenerateQRCodeAsync(data);
        }
    }
}
