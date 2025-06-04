using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace WpfApp10
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        private bool _notificationsEnabled;
        private bool _isDarkTheme = true;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool NotificationsEnabled
        {
            get => _notificationsEnabled;
            set
            {
                if (_notificationsEnabled != value)
                {
                    _notificationsEnabled = value;
                    OnPropertyChanged();
                    ShowNotificationMessage();
                }
            }
        }

        public bool IsDarkTheme
        {
            get => _isDarkTheme;
            set
            {
                if (_isDarkTheme != value)
                {
                    _isDarkTheme = value;
                    OnPropertyChanged();
                    ApplyTheme(_isDarkTheme);
                }
            }
        }

        public ICommand ClearHistoryCommand { get; }
        public ICommand LogoutCommand { get; }
        public ICommand OpenProfileSettingsCommand { get; }

        public Action CloseWindowAction { get; set; }
        public Action OpenProfileSettingsAction { get; set; }
        public Action OpenAuthWindowAction { get; set; }

        public SettingsViewModel()
        {
            ClearHistoryCommand = new RelayCommand(_ => ClearHistory());
            LogoutCommand = new RelayCommand(_ => Logout());
            OpenProfileSettingsCommand = new RelayCommand(_ => OpenProfileSettings());
        }

        private void ShowNotificationMessage()
        {
            MessageBox.Show(
                NotificationsEnabled ? "Уведомления включены" : "Уведомления выключены",
                "Настройки", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ApplyTheme(bool isDarkTheme)
        {
            var app = Application.Current;
            if (isDarkTheme)
            {
                app.Resources["WindowBackgroundBrush"] = app.TryFindResource("DarkWindowBackground");
                app.Resources["ControlBackgroundBrush"] = app.TryFindResource("DarkControlBackground");
                app.Resources["TextForegroundBrush"] = app.TryFindResource("DarkTextForeground");
                app.Resources["ButtonBackgroundBrush"] = app.TryFindResource("DarkButtonBackground");
                app.Resources["MessageBackgroundBrush"] = app.TryFindResource("DarkMessageBackground");
                app.Resources["ChatBackgroundBrush"] = app.TryFindResource("DarkChatBackground");
                app.Resources["ListBoxBackgroundBrush"] = app.TryFindResource("DarkListBoxBackground");
            }
            else
            {
                app.Resources["WindowBackgroundBrush"] = app.TryFindResource("LightWindowBackground");
                app.Resources["ControlBackgroundBrush"] = app.TryFindResource("LightControlBackground");
                app.Resources["TextForegroundBrush"] = app.TryFindResource("LightTextForeground");
                app.Resources["ButtonBackgroundBrush"] = app.TryFindResource("LightButtonBackground");
                app.Resources["MessageBackgroundBrush"] = app.TryFindResource("LightMessageBackground");
                app.Resources["ChatBackgroundBrush"] = app.TryFindResource("LightChatBackground");
                app.Resources["ListBoxBackgroundBrush"] = app.TryFindResource("LightListBoxBackground");
            }
        }

        private void ClearHistory()
        {
            var result = MessageBox.Show("Вы уверены, что хотите очистить историю?",
                                         "Очистка истории",
                                         MessageBoxButton.YesNo,
                                         MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                MessageBox.Show("История очищена", "Настройки", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Logout()
        {
            var result = MessageBox.Show("Вы действительно хотите выйти из аккаунта?",
                                         "Выход",
                                         MessageBoxButton.YesNo,
                                         MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                OpenAuthWindowAction?.Invoke();
                CloseWindowAction?.Invoke();
            }
        }

        private void OpenProfileSettings()
        {
            OpenProfileSettingsAction?.Invoke();
        }

        protected void OnPropertyChanged([CallerMemberName] string propName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }
}
