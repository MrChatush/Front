using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BroMessenger
{
    public partial class AuthWindow : Window
    {
        public AuthWindow()
        {
            InitializeComponent();

            // Обработчики для TextBox с именем пользователя
            UsernameBox.GotFocus += (s, e) =>
            {
                if (UsernameBox.Text == "Имя пользователя")
                {
                    UsernameBox.Text = "";
                    UsernameBox.Foreground = Brushes.White;
                }
            };

            UsernameBox.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(UsernameBox.Text))
                {
                    UsernameBox.Text = "Имя пользователя";
                    UsernameBox.Foreground = (Brush)new BrushConverter().ConvertFrom("#FFAAAAAA");
                }
            };

            // Обработчики для PasswordBox
            PasswordBox.GotFocus += (s, e) =>
            {
                PasswordBox.Foreground = Brushes.White;
                PasswordPlaceholder.Visibility = Visibility.Collapsed;
            };

            PasswordBox.LostFocus += (s, e) =>
            {
                if (string.IsNullOrEmpty(PasswordBox.Password))
                {
                    PasswordBox.Foreground = (Brush)new BrushConverter().ConvertFrom("#FFAAAAAA");
                    PasswordPlaceholder.Visibility = Visibility.Visible;
                }
            };
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (PasswordBox.Password.Length > 0)
            {
                PasswordPlaceholder.Visibility = Visibility.Collapsed;
            }
            else
            {
                PasswordPlaceholder.Visibility = Visibility.Visible;
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            // Создаем и показываем MainWindow
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();

            // Закрываем текущее окно авторизации
            this.Close();
        }
    }
}