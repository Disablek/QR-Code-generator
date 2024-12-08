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
    public partial class PhotoPage : Page
    {
        public PhotoPage()
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
                    IsImage(file);
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
                IsImage(filePath);
            }
        }

        private bool IsImage(string filePath)
        {
            fileImage.Source = new BitmapImage(new Uri(filePath));
            return true;
        }

    }
}
