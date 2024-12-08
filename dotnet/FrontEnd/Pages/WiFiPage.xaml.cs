using System;
using System.Collections.Generic;
using System.Globalization;
using System.Diagnostics;
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

namespace FrontEnd
{
    /// <summary>
    /// Логика взаимодействия для WiFiPage.xaml
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
            TextBox textBox = sender as TextBox;
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
            TextBox textBox = sender as TextBox;
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
                string ssid = GetCurrentSSID();
                string encryptionType = GetEncryptionType();
                bool isHidden = IsNetworkHidden();
                string password = GetWifiPassword(ssid);

                // Автоматическое заполнение полей
                userInput.Text = ssid;
                watermarkedTxt.Visibility = Visibility.Collapsed;
                userInput.Visibility = Visibility.Visible;

                // Установка шифрования
                if (encryptionType.Contains("WPA"))
                {
                    WPA_WPA2.IsChecked = true;
                }
                else if (encryptionType.Contains("WEP"))
                {
                    WEP.IsChecked = true;
                }
                else
                {
                    None.IsChecked = true;
                }

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
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return output;
        }

        private string GetCurrentSSID()
        {
            string output = RunNetshCommand("wlan show interfaces");
            foreach (var line in output.Split('\n'))
            {
                if (line.TrimStart().StartsWith("SSID"))
                {
                    return line.Split(':')[1].Trim();
                }
            }
            throw new Exception("Не удалось найти SSID текущей сети");
        }

        private string GetEncryptionType()
        {
            string output = RunNetshCommand("wlan show interfaces");
            foreach (var line in output.Split('\n'))
            {
                if (line.TrimStart().StartsWith("Authentication"))
                {
                    return line.Split(':')[1].Trim();
                }
            }
            throw new Exception("Не удалось найти тип шифрования");
        }

        private bool IsNetworkHidden()
        {
            string output = RunNetshCommand("wlan show networks mode=bssid");
            string ssid = GetCurrentSSID();
            foreach (var line in output.Split('\n'))
            {
                if (line.Contains(ssid))
                {
                    return false;
                }
            }
            return true;
        }

        private string GetWifiPassword(string ssid)
        {
            string output = RunNetshCommand($"wlan show profile name=\"{ssid}\" key=clear");
            foreach (var line in output.Split('\n'))
            {
                if (line.TrimStart().StartsWith("Key Content"))
                {
                    return line.Split(':')[1].Trim();
                }
            }
            return "Пароль не найден или сеть не использует пароль";
        }
    }
}
