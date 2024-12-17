using System.Windows;
using System.Windows.Controls;

namespace FrontEnd
{
    /// <summary>
    ///     Логика взаимодействия для TextPage.xaml
    /// </summary>
    public partial class TextPage : Page
    {
        public TextPage()
        {
            InitializeComponent();
        }

        private void userInput_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(userInput.Text))
            {
                userInput.Visibility = Visibility.Collapsed;
                watermarkedTxt.Visibility = Visibility.Visible;
            }
        }

        private void watermarkedTxt_GotFocus(object sender, RoutedEventArgs e)
        {
            watermarkedTxt.Visibility = Visibility.Collapsed;
            userInput.Visibility = Visibility.Visible;
            userInput.Focus();
        }
    }
}