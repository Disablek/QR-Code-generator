using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;

namespace FrontEnd
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _currentLanguage = "ru";
        public MainWindow()
        {
            InitializeComponent();
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
                Xceed.Wpf.Toolkit.MessageBox.Show($"{QRERROR}","Ошибка!");
                return;
            }
            string pageName = activePage.GetType().Name;
            if (pageName == "TextPage" || pageName == "LinkPage")
            {
                // Ищем элемент на активной странице по имени
                TextBox myTextBox = activePage.FindName("myTextBox") as TextBox;



            }
            else if (pageName == "WiFiPage")
            {

            }
            else if (pageName == "FilePage")
            {

            }
            else if (pageName == "PhotoPage")
            {

            }
            else
            {

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
    }
}
