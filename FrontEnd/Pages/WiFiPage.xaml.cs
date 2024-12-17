using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace FrontEnd
{
    /// <summary>
    ///     Логика взаимодействия для WiFiPage.xaml
    /// </summary>
    public partial class WiFiPage : Page
    {
        public WiFiPage()
        {
            InitializeComponent();
            WPA_WPA2.IsChecked = true;
        }

        private void userInput_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox.Name == "userInput")
            {
                if (string.IsNullOrEmpty(userInput.Text))
                {
                    userInput.Visibility = Visibility.Collapsed;
                    watermarkedTxt.Visibility = Visibility.Visible;
                }
            }
            else if (textBox.Name == "PasswordInput")
            {
                if (string.IsNullOrEmpty(PasswordInput.Text))
                {
                    PasswordInput.Visibility = Visibility.Collapsed;
                    watermarkedPassword.Visibility = Visibility.Visible;
                }
            }
        }

        private void watermarkedTxt_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox.Name == "watermarkedTxt")
            {
                watermarkedTxt.Visibility = Visibility.Collapsed;
                userInput.Visibility = Visibility.Visible;
                userInput.Focus();
            }
            else if (textBox.Name == "watermarkedPassword")
            {
                watermarkedPassword.Visibility = Visibility.Collapsed;
                PasswordInput.Visibility = Visibility.Visible;
                PasswordInput.Focus();
            }
        }

        private void EncryptionOption_Checked(object sender, RoutedEventArgs e)
        {
            if (sender == None)
            {
                // Скрываем элементы
                PasswordInput.Visibility = Visibility.Collapsed;
                watermarkedPassword.Visibility = Visibility.Collapsed;
                Password.Visibility = Visibility.Collapsed;
            }
            else
            {
                // Показываем элементы
                PasswordInput.Visibility = Visibility.Visible;
                watermarkedPassword.Visibility = Visibility.Visible;
                Password.Visibility = Visibility.Visible;
            }
        }

        private void GetWifiInfoButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Получаем информацию о Wi-Fi
                var ssid = GetCurrentSSID();
                var encryptionType = GetEncryptionType();
                var isHidden = IsNetworkHidden();
                var password = GetWifiPassword(ssid);

                // Автоматическое заполнение полей
                userInput.Text = ssid;
                watermarkedTxt.Visibility = Visibility.Collapsed;
                userInput.Visibility = Visibility.Visible;

                // Установка шифрования
                if (encryptionType.Contains("WPA"))
                    WPA_WPA2.IsChecked = true;
                else if (encryptionType.Contains("WEP"))
                    WEP.IsChecked = true;
                else
                    None.IsChecked = true;

                // Скрытая сеть
                IsHidden.IsChecked = isHidden;

                // Пароль
                PasswordInput.Text = password;
                watermarkedPassword.Visibility = Visibility.Collapsed;
                PasswordInput.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }


        private string RunNetshCommand(string arguments)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "netsh",
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            var output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return output;
        }

        private string GetCurrentSSID()
        {
            var output = RunNetshCommand("wlan show interfaces");
            foreach (var line in output.Split('\n'))
                if (line.TrimStart().StartsWith("SSID"))
                    return line.Split(':')[1].Trim();

            throw new Exception("Не удалось найти SSID текущей сети");
        }

        private string GetEncryptionType()
        {
            var output = RunNetshCommand("wlan show interfaces");
            foreach (var line in output.Split('\n'))
                if (line.TrimStart().StartsWith("Authentication"))
                    return line.Split(':')[1].Trim();

            throw new Exception("Не удалось найти тип шифрования");
        }

        private bool IsNetworkHidden()
        {
            var output = RunNetshCommand("wlan show networks mode=bssid");
            var ssid = GetCurrentSSID();
            foreach (var line in output.Split('\n'))
                if (line.Contains(ssid))
                    return false;

            return true;
        }

        private string GetWifiPassword(string ssid)
        {
            var output = RunNetshCommand($"wlan show profile name=\"{ssid}\" key=clear");
            foreach (var line in output.Split('\n'))
                if (line.TrimStart().StartsWith("Key Content"))
                    return line.Split(':')[1].Trim();

            return "Пароль не найден или сеть не использует пароль";
        }
    }
}