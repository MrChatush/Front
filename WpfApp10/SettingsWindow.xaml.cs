using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace BroMessenger
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();

            // Установка начальной темы (темная по умолчанию)
            ThemeToggle.IsChecked = true;
            ApplyTheme(isDarkTheme: true);
        }

        private void NotificationsToggle_Checked(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Уведомления включены", "Настройки", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void NotificationsToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Уведомления выключены", "Настройки", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ThemeToggle_Checked(object sender, RoutedEventArgs e)
        {
            ApplyTheme(isDarkTheme: true);
        }

        private void ThemeToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            ApplyTheme(isDarkTheme: false);
        }

        private void ApplyTheme(bool isDarkTheme)
        {
            if (isDarkTheme)
            {
                // Применяем темную тему
                Application.Current.Resources["WindowBackgroundBrush"] = FindResource("DarkWindowBackground");
                Application.Current.Resources["ControlBackgroundBrush"] = FindResource("DarkControlBackground");
                Application.Current.Resources["TextForegroundBrush"] = FindResource("DarkTextForeground");
                Application.Current.Resources["ButtonBackgroundBrush"] = FindResource("DarkButtonBackground");
                Application.Current.Resources["MessageBackgroundBrush"] = FindResource("DarkMessageBackground");
                Application.Current.Resources["ChatBackgroundBrush"] = FindResource("DarkChatBackground");
                Application.Current.Resources["ListBoxBackgroundBrush"] = FindResource("DarkListBoxBackground");
            }
            else
            {
                // Применяем светлую тему
                Application.Current.Resources["WindowBackgroundBrush"] = FindResource("LightWindowBackground");
                Application.Current.Resources["ControlBackgroundBrush"] = FindResource("LightControlBackground");
                Application.Current.Resources["TextForegroundBrush"] = FindResource("LightTextForeground");
                Application.Current.Resources["ButtonBackgroundBrush"] = FindResource("LightButtonBackground");
                Application.Current.Resources["MessageBackgroundBrush"] = FindResource("LightMessageBackground");
                Application.Current.Resources["ChatBackgroundBrush"] = FindResource("LightChatBackground");
                Application.Current.Resources["ListBoxBackgroundBrush"] = FindResource("LightListBoxBackground");
            }
        }

        private void ClearHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите очистить историю?",
                                                   "Очистка истории",
                                                   MessageBoxButton.YesNo,
                                                   MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                MessageBox.Show("История очищена", "Настройки", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы действительно хотите выйти из аккаунта?",
                                                   "Выход",
                                                   MessageBoxButton.YesNo,
                                                   MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // Закрываем все окна и открываем окно авторизации
                var authWindow = new AuthWindow();
                authWindow.Show();

                foreach (Window window in Application.Current.Windows)
                {
                    if (window != authWindow)
                        window.Close();
                }
            }
        }

        private void ProfileSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            var profileWindow = new ProfileSettingsWindow();
            profileWindow.Owner = this;
            profileWindow.ShowDialog();
        }
    }
}