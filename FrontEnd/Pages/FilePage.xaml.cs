using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FrontEnd
{
    public partial class FilePage : Page
    {
        public FilePage()
        {
            InitializeComponent();
        }

        private void DropZone_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                fileInfoText.Background = new SolidColorBrush(Colors.LightBlue);
                e.Effects = DragDropEffects.Copy;
            }
        }

        private void DropZone_DragLeave(object sender, DragEventArgs e)
        {
            fileInfoText.Background = new SolidColorBrush(Colors.LightGray);
        }

        private void DropZone_Drop(object sender, DragEventArgs e)
        {
            fileInfoText.Background = new SolidColorBrush(Colors.LightGray);
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (var file in files)
                {
                    DisplayFileInfo(file);
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Все файлы (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                DisplayFileInfo(filePath);
            }
        }

        private void DisplayFileInfo(string filePath)
        {
            if (File.Exists(filePath))
            {
                // Получаем имя файла и его размер
                string fileName = Path.GetFileName(filePath);
                long fileSize = new FileInfo(filePath).Length;

                // Обновляем текстовое поле с информацией о файле
                fileInfoText.Text = $"Имя файла: {fileName}\nРазмер: {fileSize / 1024} KB";

                // Проверяем, если файл - изображение, отображаем его
                if (IsImage(filePath))
                {
                    try
                    {
                        using (var stream = File.OpenRead(filePath))
                        {
                            var image = new BitmapImage();
                            image.BeginInit();
                            image.StreamSource = stream;
                            image.CacheOption = BitmapCacheOption.OnLoad;
                            image.EndInit();

                            fileImage.Source = image;
                            fileImage.Visibility = Visibility.Visible; // Показываем изображение
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка загрузки изображения: " + ex.Message);
                        fileImage.Visibility = Visibility.Hidden;
                    }
                }
                else
                {
                    fileImage.Source = null; // Скрываем изображение для не-изображений
                    fileImage.Visibility = Visibility.Hidden; // Скрываем изображение

                    // Показать иконку для других типов файлов
                    DisplayFileIcon(filePath);
                }
            }
        }

        private void DisplayFileIcon(string filePath)
        {
            string extension = Path.GetExtension(filePath).ToLower();

            // Добавьте логику для отображения иконок по типам файлов
            if (extension == ".txt")
            {
                fileInfoText.Text += "\nТип файла: Текстовый файл";
                fileImage.Source = new BitmapImage(new Uri("Resources/text_icon.png", UriKind.Relative));

            }
            else if (extension == ".mp3" || extension == ".wav")
            {
                fileInfoText.Text += "\nТип файла: Аудио";
                fileImage.Source = new BitmapImage(new Uri(@"Resources\audio_icon.png", UriKind.Relative));
            }
            else if (extension == ".mp4" || extension == ".avi")
            {
                fileInfoText.Text += "\nТип файла: Видео";
                fileImage.Source = new BitmapImage(new Uri("Resources/video_icon.png", UriKind.Relative));
            }
            else
            {
                fileImage.Source = new BitmapImage(new Uri("Resources/unknown_icon.png", UriKind.Relative));
            }
            fileImage.Visibility = Visibility.Visible;
        }

        private bool IsImage(string filePath)
        {
            string[] imageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff", ".webp" };

            string extension = Path.GetExtension(filePath).ToLower();

            // Проверяем, является ли расширение файла расширением изображения
            return imageExtensions.Contains(extension);
        }

    }
}
