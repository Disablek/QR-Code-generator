using System;
using System.IO; 
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace FrontEnd
{
    public partial class PhotoPage : Page
    {
        public PhotoPage()
        {
            InitializeComponent();
        }
        private void Window_DragEnter(object sender, DragEventArgs e)
        {
            // Проверяем, содержит ли объект данные в виде файлов
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Получаем массив файлов из события
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                if (files != null && files.Length > 0)
                {
                    // Получаем полный путь к первому файлу
                    string filePath = files[0];

                    // Получаем путь без префикса file://
                    var fileLinkUri = Path.GetFullPath(filePath);

                    // Устанавливаем ссылку на файл
                    fileLink.Content = fileLinkUri;

                    // Дополнительная обработка файла
                    ProcessFile(filePath);
                }
            }
        }



        private void ProcessFile(string filePath)
        {
            // Логика обработки файла (например, вывод пути в TextBlock или отправка на сервер)
            Console.WriteLine($"Обрабатывается файл: {filePath}");
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
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (var file in files) IsImage(file);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Все файлы (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                var filePath = openFileDialog.FileName;
                IsImage(filePath);
            }
        }

        private bool IsImage(string filePath)
        {
            fileImage.Source = new BitmapImage(new Uri(filePath));
            fileLink.Content = filePath;
            return true;
        }
    }
}