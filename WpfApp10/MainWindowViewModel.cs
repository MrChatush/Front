
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;


namespace WpfApp10
{
    public class MainWindowViewModel : ObservableObject // ObservableObject реализует INotifyPropertyChanged (можно реализовать вручную)
    {
        private  HttpClient _httpClient;
        private HubConnection _hubConnection;

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
                    _ = InitializeSignalRAsync();
                }
            }
        }

        private string _currentChatName = "1";
        public string CurrentChatName
        {
            get => _currentChatName;
            set { _currentChatName = value; OnPropertyChanged(); }
        }

        private string _messageText = "";
        public string MessageText
        {
            get => _messageText;
            set { _messageText = value; OnPropertyChanged();}
        }

        public ICommand OpenSettingsCommand { get; }
        public ICommand AddChatCommand { get; }
        public ICommand SendMessageCommand { get; }
        public ICommand LoadChatsCommand { get; }

        public MainWindowViewModel(string token)
        {
            InitializeHttpClient();
            Token = token;
            UpdateHttpClientAuthorization();
            SendMessageCommand = new RelayCommand(async _ => await SendMessageAsync(), _ => !string.IsNullOrWhiteSpace(MessageText));
            LoadChatsCommand = new RelayCommand(async _ => await LoadChatsAsync());
            OpenSettingsCommand = new RelayCommand(_ => OpenSettings());
            AddChatCommand = new RelayCommand(_ => AddChat());
            _ = LoadChatsAsync();
        }

        private void OpenSettings()
        {
            var win = new SettingsWindow();
            win.ShowDialog();
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

            _hubConnection.On<MessageDto>("ReceiveMessage", message =>
            {
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

                int userId = GetUserIdFromToken(Token); // Реализуйте метод для получения userId из токена

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
                // Ищем клейм с типом "userId"
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

            return 0; // Возвращаем 0, если не удалось извлечь userId
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
                    foreach (var msg in messages)
                        Messages.Add(msg);
                });

                await _hubConnection.InvokeAsync("JoinRoom", chatId);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке сообщений или присоединении к чату: " + ex.Message);
            }
        }
        //private async Task ConnectToHubAsync()
        //{
        //    _hubConnection = new HubConnectionBuilder().WithUrl("https://localhost:7000/chat", options =>
        //    {
        //            options.AccessTokenProvider = () => Task.FromResult(Token);
        //        }).WithAutomaticReconnect().Build();

        //    //_hubConnection.On<MessageDto>("ReceiveMessage", message =>
        //    //{
        //    //    if (SelectedChat != null && message.ChatId == SelectedChat.Id)
        //    //    {
        //    //        Application.Current.Dispatcher.Invoke(() =>
        //    //        {
        //    //            Messages.Add(message);
        //    //        });
        //    //    }
        //    //});

        //    try
        //    {
        //        await _hubConnection.StartAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Ошибка подключения к SignalR: " + ex.Message);
        //    }
        //}

        private async Task SendMessageAsync()
        {
            if (SelectedChat == null || string.IsNullOrWhiteSpace(MessageText))
                return;

            try
            {
                //if (_hubConnection.State != HubConnectionState.Connected)
                //{
                //    await _hubConnection.StartAsync();
                //}
                await _hubConnection.InvokeAsync("SendMessage", SelectedChat.Id, MessageText);
                MessageText = string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка отправки сообщения:\n{ex.GetType().Name}\n{ex.Message}\n{ex.StackTrace}{ex.Source}");
            }

        }
        public class ChatDto
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public bool IsGroup { get; set; }
        }

        public class MessageDto
        {
            public int Id { get; set; }
            public int ChatId { get; set; }
            public int SenderId { get; set; }
            public string Text { get; set; }
            public DateTime SentAt { get; set; }
            public bool IsRead { get; set; }
        }
    }
}
