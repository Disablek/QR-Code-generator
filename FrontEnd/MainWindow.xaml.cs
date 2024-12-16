using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using FrontEnd.Pages;
using Serilog;
using Serilog.Sinks.SystemConsole;
using Serilog.Sinks.File;
using Image = System.Windows.Controls.Image;
using MessageBox = Xceed.Wpf.Toolkit.MessageBox;

namespace FrontEnd
{
    public partial class MainWindow : Window
    {
        private const int DebounceInterval = 150; // Задержка в миллисекундах перед вызовом метода

        private readonly DispatcherTimer _debounceTimer;
        private readonly HttpClient _httpClient; // Убираем создание нового клиента здесь
        private string _currentLanguage = "ru";
        private Bitmap QRImage;

        public MainWindow()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(@"C:\\Users\\Disable\\source\\repos\\Disablek\\TEsTS\\QR-Code-generator\\FrontEnd\\logs\\log.txt", rollingInterval: RollingInterval.Day,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}")
                .WriteTo.Console(
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}")
                .CreateLogger();

            try
            {
                Log.Information("Приложение запущено в {Time}", DateTime.Now);

                // Эмуляция работы с базой данных
                var result = Task.Run(() => "Пример результата");
                Log.Information("Результат работы с базой данных: {Result}", result);

                // Исключение для демонстрации логирования ошибок
                throw new Exception("Тестовая ошибка");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Произошла ошибка в процессе обработки запроса: {RequestId}", Guid.NewGuid());
            }
            finally
            {
                Log.CloseAndFlush();
            }

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
            Log.Information("Пользователь нажал кнопку для генерации QR-кода с текстом.");
            ContentFrame.Navigate(new TextPage());
        }

        private void ButtonQRWiFi_Click(object sender, RoutedEventArgs e)
        {
            Log.Information("Пользователь нажал кнопку для генерации QR-кода для WiFi.");
            ContentFrame.Navigate(new WiFiPage());
        }

        private void ButtonQRFile_Click(object sender, RoutedEventArgs e)
        {
            Log.Information("Пользователь нажал кнопку для генерации QR-кода для файла.");
            ContentFrame.Navigate(new FilePage());
        }

        private void ButtonQRURL_Click(object sender, RoutedEventArgs e)
        {
            Log.Information("Пользователь нажал кнопку для генерации QR-кода для URL.");
            ContentFrame.Navigate(new LinkPage());
        }

        private void ButtonQRPhoto_Click(object sender, RoutedEventArgs e)
        {
            Log.Information("Пользователь нажал кнопку для генерации QR-кода с фото.");
            ContentFrame.Navigate(new PhotoPage());
        }

        private async void ButtonGenerateQR_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Log.Information("Запущена генерация QR-кода.");
                // Получаем ссылку на активную страницу
                var activePage = ContentFrame.Content as Page;
                var QRERROR = Application.Current.Resources["QRCodeGenerateError"] as string;
                if (string.IsNullOrEmpty(QRERROR)) QRERROR = "Сообщение об ошибке не найдено"; // Резервное сообщение

                if (activePage == null)
                {
                    Log.Warning("Активная страница не найдена.");
                    MessageBox.Show($"{QRERROR}", "Ошибка!");
                    return;
                }

                var pageName = activePage.GetType().Name;
                string data = null;
                var qrCodeClient = new QRCodeClient(_httpClient);

                // Запускаем анимацию ожидания
                ShowWaitingAnimationAsync(true, true);
                if (pageName == "TextPage" || pageName == "LinkPage")
                {
                    // Ищем элемент на активной странице по имени
                    var myTextBox = activePage.FindName("userInput") as TextBox;
                    data = myTextBox.Text;
                }
                else if (pageName == "WiFiPage")
                {
                    Log.Information("Генерация QR-кода для WiFi.");
                    // Ищем элементы TextBox на активной странице
                    var ssidInput = (activePage.FindName("userInput") as TextBox).Text;
                    var passwordInput = (activePage.FindName("PasswordInput") as TextBox).Text;

                    var radioButton1 = activePage.FindName("WPA_WPA2") as RadioButton;
                    var radioButton2 = activePage.FindName("WEP") as RadioButton;
                    var radioButton3 = activePage.FindName("None") as RadioButton;
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

                    // Генерация QR-кода через API
                    data = await qrCodeClient.GenerateWiFiLinkAsync(ssidInput, passwordInput, encryption);
                }
                else if (pageName == "FilePage")
                {
                    var fileName = (activePage.FindName("filePath") as Label).Content.ToString();
                    var daysTillDeleting = int.Parse((activePage.FindName("daysCount") as TextBox).Text);

                    // Запускаем анимацию ожидания
                    ShowWaitingAnimationAsync(true, true);
                    data = await qrCodeClient.UploadFileAsync(fileName, daysTillDeleting);
                }
                else if (pageName == "PhotoPage")
                {
                    var fileLink = (activePage.FindName("fileLink") as Label).Content.ToString();
                    ShowWaitingAnimationAsync(true, true);
                    data = await qrCodeClient.GenerateImageLinkAsync(fileLink);
                } 
                else
                {
                    ShowWaitingAnimationAsync(false, true);
                    return;
                }

                ShowWaitingAnimationAsync(false, true);

                // Если данных нет, то выводим сообщение
                if (string.IsNullOrEmpty(data))
                {
                    Log.Warning("Не получены данные для генерации QR-кода.");
                    MessageBox.Show("Пожалуйста, введите данные для генерации QR-кода.", "Ошибка!");
                    return;
                }

                // Запускаем анимацию ожидания
                ShowWaitingAnimationAsync(true, true);

                // Генерируем QR-код
                await GenerateQRCode(data, qrCodeClient);
                // Останавливаем анимацию после завершения генерации QR-кода

                ShowWaitingAnimationAsync(false, true);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка при генерации QR-кода.");
                MessageBox.Show($"{ex}", "Ошибка!");
                ShowWaitingAnimationAsync(false, true);
            }
        }

        public void ShowWaitingAnimationAsync(bool action, bool which)
        {
            if (which)
            {
                if (action)
                    // Запуск анимации
                    AnimationImage.Visibility = Visibility.Visible;
                else
                    // Остановка анимации
                    AnimationImage.Visibility = Visibility.Hidden;
            }
            else
            {
                if (action)
                    // Запуск анимации
                    AnimationImage2.Visibility = Visibility.Visible;
                else
                    // Остановка анимации
                    AnimationImage2.Visibility = Visibility.Hidden;
            }
        }


        private void ChangeDictianory(object sender, RoutedEventArgs e)
        {
            // Очистить текущие словари ресурсов
            Resources.MergedDictionaries.Clear();

            // Определить путь к новому словарю
            var newDictionaryPath = _currentLanguage == "ru"
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

        public void DisplayImageFromFile(string filePath, Image imageControl)
        {
            try
            {
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri(filePath, UriKind.Absolute); // Используем абсолютный путь
                bitmapImage.EndInit();

                // Обновление UI в главном потоке
                Application.Current.Dispatcher.Invoke(() =>
                {
                    imageControl.Source = bitmapImage;
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка при обработке изображения.");
                MessageBox.Show($"Ошибка при отображении изображения: {ex.Message}", "Ошибка!");
            }
        }


        private async Task GenerateQRCode(string data, QRCodeClient qrCodeClient)
        {
            try
            {
                QRImage = await qrCodeClient.GenerateQRCodeAsync(data);

                if (QRImage != null)
                {
                    // Путь для сохранения изображения
                    string filePath = @"C:\\Users\\Disable\\source\\repos\\Disablek\\TEsTS\\QR-Code-generator\\FrontEnd\\QRCodeTest.png";

                    // Сохраняем изображение на диск
                    SaveQRCodeImage(QRImage, filePath);

                    // Отображаем изображение из файла
                    DisplayImageFromFile(filePath, QRCodeImageContainer);

                    colorPicker.Visibility = Visibility.Visible;
                }
                else
                {
                    MessageBox.Show("Пусто", "Ошибка!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при генерации QR-кода: {ex.Message}", "Ошибка!");
            }
        }



        private void SaveQRCodeImage(System.Drawing.Bitmap qrImage, string filePath)
        {
            try
            {
                qrImage.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
                Log.Information($"Изображение QR-кода сохранено в: {filePath}");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка при сохранении изображения QR-кода.");
            }
        }


        // Обработчик изменения цвета в ColorPicker
        private void ColorPicker_SelectedColorChanged(object sender,
            RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
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
                var drawingColor = System.Drawing.Color.FromArgb(
                    selectedColor.Value.A,  // Альфа-канал
                    selectedColor.Value.R,  // Красный
                    selectedColor.Value.G,  // Зеленый
                    selectedColor.Value.B   // Синий
                );

                // Вызовите метод, который нужно вызвать при изменении цвета
                ChangeColor(drawingColor);
            }
        }



        // Метод, который будет вызван после изменения цвета
        private void ChangeColor(System.Drawing.Color newColor)
        {
            if (QRImage == null) return;

            // Создаем новый объект Bitmap на основе исходного изображения
            using (var newBitmap = new System.Drawing.Bitmap(QRImage))
            {
                // Получаем данные изображения для прямого доступа к пикселям
                var rect = new System.Drawing.Rectangle(0, 0, newBitmap.Width, newBitmap.Height);
                var bitmapData = newBitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, newBitmap.PixelFormat);

                // Получаем указатель на массив данных пикселей
                IntPtr ptr = bitmapData.Scan0;

                // Количество байтов, занимаемых одним пикселем (в зависимости от формата)
                int bytesPerPixel = System.Drawing.Bitmap.GetPixelFormatSize(newBitmap.PixelFormat) / 8;

                // Массив для хранения данных пикселей
                byte[] pixels = new byte[bitmapData.Stride * newBitmap.Height];

                // Копируем данные пикселей в массив
                System.Runtime.InteropServices.Marshal.Copy(ptr, pixels, 0, pixels.Length);

                // Проходим по каждому пикселю
                for (int y = 0; y < newBitmap.Height; y++)
                {
                    for (int x = 0; x < newBitmap.Width; x++)
                    {
                        // Индекс пикселя в массиве
                        int pixelIndex = (y * bitmapData.Stride) + (x * bytesPerPixel);

                        // Извлекаем цвет пикселя
                        byte blue = pixels[pixelIndex];
                        byte green = pixels[pixelIndex + 1];
                        byte red = pixels[pixelIndex + 2];

                        // Если пиксель черный (или близкий к черному), меняем его на новый цвет
                        if (red == 0 && green == 0 && blue == 0)
                        {
                            // Заменяем на новый цвет
                            pixels[pixelIndex] = newColor.B;     // Синий канал
                            pixels[pixelIndex + 1] = newColor.G; // Зеленый канал
                            pixels[pixelIndex + 2] = newColor.R; // Красный канал
                        }
                    }
                }

                // Копируем измененные данные обратно в изображение
                System.Runtime.InteropServices.Marshal.Copy(pixels, 0, ptr, pixels.Length);

                // Разблокируем доступ к пикселям
                newBitmap.UnlockBits(bitmapData);

                // Генерация временного имени для файла
                string tempFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".png");

                // Сохраняем измененное изображение во временный файл
                newBitmap.Save(tempFilePath, System.Drawing.Imaging.ImageFormat.Png);

                // Обновляем отображение изображения в интерфейсе
                DisplayImageFromFile(tempFilePath, QRCodeImageContainer);
            }
        }






        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, существует ли Frame на странице
            if (ContentFrame2.Visibility == Visibility.Visible)
            {
                // Если Frame видим, скрываем его
                ContentFrame2.Visibility = Visibility.Collapsed;
            }
            else
            {
                //// Показываем Frame
                ContentFrame2.Visibility = Visibility.Visible;
                var page = new HistoryPage(this);
                ContentFrame2.Navigate(page);

                await page.LoadQRCodeHistory(); // Загрузка первых 20 записей
            }
        }
    }
}