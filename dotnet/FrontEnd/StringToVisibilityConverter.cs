using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FrontEnd
{
    public static class WatermarkBehavior
    {
        public static readonly DependencyProperty WatermarkProperty =
            DependencyProperty.RegisterAttached("Watermark", typeof(string), typeof(WatermarkBehavior), new PropertyMetadata(string.Empty, OnWatermarkChanged));

        public static string GetWatermark(DependencyObject obj) => (string)obj.GetValue(WatermarkProperty);

        public static void SetWatermark(DependencyObject obj, string value) => obj.SetValue(WatermarkProperty, value);

        private static void OnWatermarkChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox textBox)
            {
                textBox.GotFocus -= RemoveWatermark;
                textBox.LostFocus -= ShowWatermark;

                if (!string.IsNullOrEmpty((string)e.NewValue))
                {
                    textBox.GotFocus += RemoveWatermark;
                    textBox.LostFocus += ShowWatermark;
                    ShowWatermark(textBox, null);
                }
            }
        }

        private static void RemoveWatermark(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox && textBox.Text == (string)textBox.GetValue(WatermarkProperty))
            {
                textBox.Text = string.Empty;
                textBox.Foreground = Brushes.Black;
            }
        }

        private static void ShowWatermark(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox && string.IsNullOrEmpty(textBox.Text))
            {
                textBox.Text = (string)textBox.GetValue(WatermarkProperty);
                textBox.Foreground = Brushes.Gray;
            }
        }
    }
}
