using System;
using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json.Linq;
using static WpfApp10.MainWindowViewModel;

namespace WpfApp10
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        private bool _notificationsEnabled;
        private bool _isDarkTheme = true;
        private readonly HubConnection _hubConnection;
        private readonly HttpClient _httpClient;
        private readonly string _token;
        public event PropertyChangedEventHandler PropertyChanged;
        private readonly Func<Task> _reloadChatMessages;
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

        public SettingsViewModel(HubConnection hubConnection, HttpClient httpClient, string token,int ChatId,Func<Task> Update)
        {
            _hubConnection = hubConnection;
            _httpClient = httpClient;
            _token = token;
            ClearHistoryCommand = new RelayCommand(_ => ClearHistory());
            LogoutCommand = new RelayCommand(_ => Logout());
            OpenProfileSettingsCommand = new RelayCommand(_ => OpenProfileSettings());
            CurrentChatId = ChatId;
            _reloadChatMessages =Update;
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
        private int _currentChatId;
        public int CurrentChatId
        {
            get => _currentChatId;
            set
            {
                if (_currentChatId != value)
                {
                    _currentChatId = value;
                    OnPropertyChanged();
                }
            }
        }


        private async void ClearHistory()
        {

            var result = MessageBox.Show("Вы уверены, что хотите очистить историю?",
                                         "Очистка истории",
                                         MessageBoxButton.YesNo,
                                         MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // Предполагается, что у вас есть текущий ChatId, например, хранится в свойстве
                    int currentChatId = CurrentChatId; // Реализуйте этот метод или передавайте chatId

                    var request = new HttpRequestMessage(HttpMethod.Delete, $"api/messages/chat/{currentChatId}");
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

                    var response = await _httpClient.SendAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("История очищена", "Настройки", MessageBoxButton.OK, MessageBoxImage.Information);
                        await _reloadChatMessages();
                        // Можно дополнительно уведомить UI или обновить список сообщений
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Ошибка при очистке истории: {errorContent}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
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
