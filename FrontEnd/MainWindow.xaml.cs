using System;
using System.Windows;
using System.Net.Http;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit;
using System.Windows.Controls;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Drawing;
using System.Windows.Threading;

namespace FrontEnd
{
    public partial class MainWindow : Window
    {
        private string _currentLanguage = "ru";
        private HttpClient _httpClient; // Убираем создание нового клиента здесь
        private byte[] QRImage;

        private DispatcherTimer _debounceTimer;
        private const int DebounceInterval = 50; // Задержка в миллисекундах перед вызовом метода

        public MainWindow()
        {
            //Console.OutputEncoding = Encoding.UTF8;
            InitializeComponent();
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5000/api/QRCode/"),
                Timeout = TimeSpan.FromMinutes(1)
            };

            var qrCodeClient = new QRCodeClient(_httpClient);

            // Настройка таймера для дебаунсинга
            _debounceTimer = new DispatcherTimer();
            _debounceTimer.Interval = TimeSpan.FromMilliseconds(DebounceInterval);
            _debounceTimer.Tick += DebounceTimer_Tick;
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

        private async void ButtonGenerateQR_Click(object sender, RoutedEventArgs e)
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
            string data = null;
            QRCodeClient qrCodeClient = new QRCodeClient(_httpClient);


            if (pageName == "TextPage" || pageName == "LinkPage")
            {
                // Ищем элемент на активной странице по имени
                TextBox myTextBox = activePage.FindName("userInput") as TextBox;
                data = myTextBox.Text.ToString();
            }
            else if (pageName == "WiFiPage")
            {
                // Ищем элементы TextBox на активной странице
                string ssidInput = (activePage.FindName("userInput") as TextBox).Text.ToString();
                string passwordInput = (activePage.FindName("PasswordInput") as TextBox).Text.ToString();

                RadioButton radioButton1 = activePage.FindName("WPA_WPA2") as RadioButton;
                RadioButton radioButton2 = activePage.FindName("WEP") as RadioButton;
                RadioButton radioButton3 = activePage.FindName("None") as RadioButton;
                string encryption = null;

                // Проверяем, какой из RadioButton выбран
                if (radioButton1 != null && radioButton1.IsChecked == true)
                {
                    encryption = "WPA/WPA2";
                }
                else if (radioButton2 != null && radioButton2.IsChecked == true)
                {
                    encryption = "WEP";
                }
                else if (radioButton3 != null && radioButton3.IsChecked == true)
                {
                    encryption = "None";
                }
                else
                {
                    Console.WriteLine("Никакая опция не выбрана");
                    return;
                }
                // Запускаем анимацию ожидания
                ShowWaitingAnimationAsync(true);
                // Генерация QR-кода через API
                data = await qrCodeClient.GenerateWiFiLinkAsync(ssidInput,passwordInput,encryption);
                ShowWaitingAnimationAsync(false);  
            }

            // Если данных нет, то выводим сообщение
            if (string.IsNullOrEmpty(data))
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("Пожалуйста, введите данные для генерации QR-кода.", "Ошибка!");
                return;
            }

            // Запускаем анимацию ожидания
            ShowWaitingAnimationAsync(true);

            // Генерируем QR-код
            await GenerateQRCode(data, qrCodeClient);
            // Останавливаем анимацию после завершения генерации QR-кода

            colorPicker.Visibility= Visibility.Visible;
            ShowWaitingAnimationAsync(false);
        }

        private void ShowWaitingAnimationAsync(bool action)
        {
            if (action)
            {
                // Запуск анимации
                AnimationImage.Visibility = Visibility.Visible;
            }
            else
            {
                // Остановка анимации
                AnimationImage.Visibility = Visibility.Hidden;

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

        // Метод для отображения изображения QR-кода
        public void DisplayImageFromByteArray(byte[] imageData, System.Windows.Controls.Image imageControl, System.Drawing.Color color)
        {
            try
            {
                // Преобразуем байты в изображение
                using (var stream = new MemoryStream(imageData))
                {
                    // Создаем Bitmap из потока
                    Bitmap bitmap = new Bitmap(stream);

                    // Обрабатываем пиксели
                    for (int y = 0; y < bitmap.Height; y++)
                    {
                        for (int x = 0; x < bitmap.Width; x++)
                        {
                            System.Drawing.Color pixelColor = bitmap.GetPixel(x, y);

                            // Проверяем, является ли пиксель черным
                            if (pixelColor.R == 0 && pixelColor.G == 0 && pixelColor.B == 0)
                            {
                                // Заменяем черный на нужный цвет
                                bitmap.SetPixel(x, y, color);
                            }
                        }
                    }

                    // Преобразуем Bitmap обратно в BitmapImage
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                        memoryStream.Seek(0, SeekOrigin.Begin);

                        BitmapImage modifiedBitmapImage = new BitmapImage();
                        modifiedBitmapImage.BeginInit();
                        modifiedBitmapImage.StreamSource = memoryStream;
                        modifiedBitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        modifiedBitmapImage.EndInit();

                        // Отображаем измененное изображение
                        imageControl.Source = modifiedBitmapImage;
                    }
                }
            }
            catch (Exception ex)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show($"Ошибка при отображении изображения: {ex.Message}", "Ошибка!");
            }
        }



        // Метод для генерации QR-кода
        private async Task GenerateQRCode(string data, QRCodeClient qrCodeClient)
        {

            try
            {
                // Генерация QR-кода через API
                var response = await qrCodeClient.GenerateQRCodeAsync(data);

                if (response.ImageBytes != null)
                {
                    QRImage = response.ImageBytes;

                    // Получаем выбранный цвет
                    var selectedColor = colorPicker.SelectedColor;
                    // Преобразуем System.Windows.Media.Color в System.Drawing.Color
                    System.Drawing.Color drawingColor = System.Drawing.Color.FromArgb(selectedColor.Value.A, selectedColor.Value.R, selectedColor.Value.G, selectedColor.Value.B);

                    // Используем DisplayImageFromByteArray для отображения изображения
                    DisplayImageFromByteArray(QRImage, QRCodeImageContainer, drawingColor);
                }
                else
                {
                    Xceed.Wpf.Toolkit.MessageBox.Show(response.Message, "Ошибка!");
                }
            }
            catch (Exception ex)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show($"Ошибка при генерации QR-кода: {ex.Message}", "Ошибка!");
            }
        }



        // Обработчик изменения цвета в ColorPicker
        private void ColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {
            // Добавляем проверку на null
            if (_debounceTimer != null)
            {
                // Останавливаем предыдущий таймер, если он был запущен
                _debounceTimer.Stop();

                // Запускаем таймер для вызова метода с задержкой
                _debounceTimer.Start();
            }
        }


        // Этот метод вызывается после заданной задержки (DebounceInterval)
        private void DebounceTimer_Tick(object sender, EventArgs e)
        {
            // Останавливаем таймер после выполнения
            _debounceTimer.Stop();

            // Получаем выбранный цвет
            var selectedColor = colorPicker.SelectedColor;

            if (selectedColor.HasValue)
            {
                // Преобразуем System.Windows.Media.Color в System.Drawing.Color
                System.Drawing.Color drawingColor = System.Drawing.Color.FromArgb(selectedColor.Value.A, selectedColor.Value.R, selectedColor.Value.G, selectedColor.Value.B);

                // Вызовите метод, который нужно вызвать при изменении цвета
                ChangeColor(drawingColor);
            }
        }


        // Метод, который будет вызван после изменения цвета
        private void ChangeColor(System.Drawing.Color newColor)
        {
            DisplayImageFromByteArray(QRImage, QRCodeImageContainer, newColor);
        }
    }
}