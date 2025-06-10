using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WpfApp10
{
    public class MainWindowViewModel : ObservableObject
    {
        private HttpClient _httpClient;
        private HubConnection _hubConnection;
        private static int _myUserId;

        public static int MyUserId => _myUserId;

        public ObservableCollection<ChatDto> Chats { get; } = new ObservableCollection<ChatDto>();
        public ObservableCollection<MessageDto> Messages { get; } = new ObservableCollection<MessageDto>();

        private ChatDto _selectedChat;
        public ChatDto SelectedChat
        {
            get => _selectedChat;
            set
            {
                if (_selectedChat != value)
                {
                    _selectedChat = value;
                    OnPropertyChanged();
                    if (_selectedChat != null)
                        _ = JoinChatAsync(_selectedChat.Id);
                }
            }
        }

        private string _token;
        public string Token
        {
            get => _token;
            set
            {
                if (_token != value)
                {
                    _token = value;
                    OnPropertyChanged();
                    UpdateHttpClientAuthorization();
                    _myUserId = GetUserIdFromToken(_token);
                    _ = InitializeSignalRAsync();
                }
            }
        }

        private string _currentChatName = "";
        public string CurrentChatName
        {
            get => _currentChatName;
            set { _currentChatName = value; OnPropertyChanged(); }
        }

        private string _messageText = "";
        public string MessageText
        {
            get => _messageText;
            set { _messageText = value; OnPropertyChanged(); }
        }

        public ICommand OpenSettingsCommand { get; }
        public ICommand AddChatCommand { get; }
        public ICommand SendMessageCommand { get; }
        public ICommand LoadChatsCommand { get; }
        public ICommand WindowClosingCommand { get; }
        public MainWindowViewModel(string token)
        {
            InitializeHttpClient();
            Token = token;
            UpdateHttpClientAuthorization();
            SendMessageCommand = new RelayCommand(async _ => await SendMessageAsync(), _ => !string.IsNullOrWhiteSpace(MessageText));
            LoadChatsCommand = new RelayCommand(async _ => await LoadChatsAsync());
            OpenSettingsCommand = new RelayCommand(_ => OpenSettings());
            AddChatCommand = new RelayCommand(_ => AddChat());
            WindowClosingCommand = new RelayCommand(_ => OnWindowClosing());
            _ = LoadChatsAsync();
        }
        private async Task OnWindowClosing()
        {
            try
            {
                // Добавляем токен в заголовок Authorization, если требуется
                if (!string.IsNullOrEmpty(_token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);
                }

                // Формируем тело запроса
                var logoutRequest = new { UserId = GetUserIdFromToken(_token) };

                // Отправляем POST-запрос на /api/auth/logout
                var response = await _httpClient.PostAsJsonAsync("/api/auth/logout", logoutRequest);

                if (response.IsSuccessStatusCode)
                {
                    // Успешный logout
                    MessageBox.Show("Вы успешно вышли из системы.");

                    // Очистка локальных данных
                    _token = null;
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Ошибка выхода из системы: {error}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при выходе из системы: {ex.Message}");
            }
        }

        private void OpenSettings()
        {
            if (_selectedChat?.Id != null)
            {
                var win = new SettingsWindow(_hubConnection, _httpClient, _token, _selectedChat.Id, UpdateMessages);
                win.ShowDialog();
            }
        }

        public async Task UpdateMessages()
        {
            if (SelectedChat == null)
                return;
            try
            {
                var messages = await _httpClient.GetFromJsonAsync<MessageDto[]>($"api/messages?chatId={SelectedChat.Id}");
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Messages.Clear();
                    foreach (var msg in messages)
                        Messages.Add(msg);
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при обновлении сообщений: " + ex.Message);
            }
        }

        private void UpdateHttpClientAuthorization()
        {
            if (!string.IsNullOrEmpty(Token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
            }
            else
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }

        private void InitializeHttpClient()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7000/")
            };
        }

        private async Task InitializeSignalRAsync()
        {
            if (_hubConnection != null)
            {
                await _hubConnection.StopAsync();
                await _hubConnection.DisposeAsync();
            }

            _hubConnection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7000/chat", options =>
                {
                    options.AccessTokenProvider = () => Task.FromResult(Token);
                })
                .WithAutomaticReconnect()
                .Build();

            _hubConnection.On<int, bool>("ReceiveUserOnlineStatus", (userId, isOnline) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var chatToUpdate = Chats.FirstOrDefault(c => !c.IsGroup && c.OtherUserId == userId);
                    if (chatToUpdate != null)
                    {
                        chatToUpdate.IsOnline = isOnline;
                    }
                });
                _ = LoadChatsAsync();
            });

            _hubConnection.On<MessageDto>("ReceiveMessage", message =>
            {
                message.SentAt = DateTime.Now;
                if (SelectedChat != null && message.ChatId == SelectedChat.Id)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Messages.Add(message);
                    });
                }
            });

            _hubConnection.On<ChatDto>("NewChatCreated", chat => {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    _ = LoadChatsAsync();
                });
            });

            //_hubConnection.On<int, int>("MessagesRead", (chatId, userId) =>
            //{
            //    Application.Current.Dispatcher.Invoke(() =>
            //    {
            //        foreach (var msg in Messages.Where(m => m.ChatId == chatId && m.SenderId != userId))
            //        {
            //            msg.IsRead = true;
            //        }
            //    });
            //});

            try
            {
                await _hubConnection.StartAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка подключения к SignalR: " + ex.Message);
            }
        }

        public async Task LoadChatsAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(Token))
                {
                    MessageBox.Show("Токен не установлен. Пожалуйста, выполните вход.");
                    return;
                }

                int userId = GetUserIdFromToken(Token);
                var chats = await _httpClient.GetFromJsonAsync<ChatDto[]>($"api/chats?userId={userId}");
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Chats.Clear();
                    foreach (var chat in chats)
                        Chats.Add(chat);
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки чатов: " + ex.Message);
            }
        }

        private int GetUserIdFromToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                return 0;

            var handler = new JwtSecurityTokenHandler();

            try
            {
                var jwtToken = handler.ReadJwtToken(token);
                var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "userId");

                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                {
                    return userId;
                }
            }
            catch
            {
                // Ошибка при парсинге токена
            }

            return 0;
        }

        private void AddChat()
        {
            var win = new AddChats(_hubConnection, _httpClient, _token);
            win.ShowDialog();
        }

        private async Task JoinChatAsync(int chatId)
        {
            try
            {
                var messages = await _httpClient.GetFromJsonAsync<MessageDto[]>($"api/messages?chatId={chatId}");
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Messages.Clear();
                    foreach (var msg in messages.OrderBy(m => m.SentAt))
                        Messages.Add(msg);
                });

                await _hubConnection.InvokeAsync("JoinRoom", chatId);
                //await _httpClient.PostAsync($"api/chats/markAsRead?chatId={chatId}", null);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке сообщений или присоединении к чату: " + ex.Message);
            }
        }

        private async Task SendMessageAsync()
        {
            if (SelectedChat == null || string.IsNullOrWhiteSpace(MessageText))
                return;

            try
            {
                await _hubConnection.InvokeAsync("SendMessage", SelectedChat.Id, MessageText);
                MessageText = string.Empty;
            }

            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка отправки сообщения:\n{ex.GetType().Name}\n{ex.Message}\n{ex.StackTrace}{ex.Source}");
            }
        }

        public class ChatDto : ObservableObject
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public bool IsGroup { get; set; }
            public string AvatarUrl { get; set; }
            private bool _isOnline;
            public bool IsOnline
            {
                get => _isOnline;
                set
                {
                    if (_isOnline != value)
                    {
                        _isOnline = value;
                        OnPropertyChanged(nameof(IsOnline));
                    }
                }
            }
            public int? OtherUserId { get; set; }
            public string LastMessage { get; set; }
            public string Time { get; set; }
        }

        public class MessageDto : ObservableObject
        {
            public int Id { get; set; }
            public int ChatId { get; set; }
            public int SenderId { get; set; }
            public string SenderAvatarUrl { get; set; }
            public string Text { get; set; }
            public DateTime SentAt { get; set; }
            public bool IsRead { get; set; }
            public string Sender { get; set; }
        }
    }

    public class BoolToReadConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? "✓✓" : "✓";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? Brushes.Blue : Brushes.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class AlignmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int senderId && parameter is int myUserId)
            {
                return senderId == myUserId ? HorizontalAlignment.Right : HorizontalAlignment.Left;
            }
            return HorizontalAlignment.Left;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MessageBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int senderId && parameter is int myUserId)
            {
                return senderId == myUserId ? new SolidColorBrush(Color.FromRgb(30, 109, 217)) :
                                             new SolidColorBrush(Color.FromRgb(60, 60, 60));
            }
            return new SolidColorBrush(Color.FromRgb(60, 60, 60));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}