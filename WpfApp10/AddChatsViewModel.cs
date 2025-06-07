using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Win32;
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

public class AddChatsViewModel : INotifyPropertyChanged
{
    private string _username;
    public event PropertyChangedEventHandler PropertyChanged;
    public string ChatTitle { get; set; }
    public string ChatAvatarPath { get; set; }
    public ICommand SelectAvatarCommand { get; }
 


    public string Username
    {
        get => _username;
        set
        {
            if (_username != value)
            {
                _username = value;
                OnPropertyChanged();
            }
        }
    }
    private readonly HubConnection _hubConnection;
    private readonly HttpClient _httpClient;
    private readonly string _token;

    public AddChatsViewModel(HubConnection hubConnection, HttpClient httpClient, string token)
    {
        _hubConnection = hubConnection;
        _httpClient = httpClient;
        _token = token;
        AddChatsCommand = new RelayCommand(async _ => await SendChatAsync());

        SelectAvatarCommand = new RelayCommand(_ => SelectAvatar());
        CloseCommand = new RelayCommand(_ => RequestClose?.Invoke());
    }
    public ICommand CloseCommand { get; }

    public ICommand AddChatsCommand { get; }

    // Событие для закрытия окна
    public event Action RequestClose;
    private async Task SendChatAsync()
    {
        try
        {
            //if (_hubConnection.State != HubConnectionState.Connected)
            //{
            //    await _hubConnection.StartAsync();
            //}
            var user1 = GetUserIdFromToken(_token);
            var user2 = await GetUserIdByUsernameAsync(Username);
            await _hubConnection.InvokeAsync("CreatePrivateChat", user1, user2,Username);
            //RequestClose.Invoke();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка отправки сообщения:\n{ex.GetType().Name}\n{ex.Message}\n{ex.StackTrace}{ex.Source}");
        }

    }
    private async Task<int?> GetUserIdByUsernameAsync(string username)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/users/by-username?username={Uri.EscapeDataString(username)}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                // Пользователь не найден — возвращаем null
                return null;
            }

            response.EnsureSuccessStatusCode();

            var userId = await response.Content.ReadFromJsonAsync<int>();
            return userId;
        }
        catch (Exception ex)
        {
            // Можно логировать ошибку ex, если нужно
            // Возвращаем null, чтобы не считать 0 валидным Id
            return null;
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

    private void SelectAvatar()
    {
        var openFileDialog = new OpenFileDialog()
        {
            Title = "Выберите файл аватара",
            Filter = "Изображения (*.png;*.jpg;*.jpeg;*.bmp)|*.png;*.jpg;*.jpeg;*.bmp|Все файлы (*.*)|*.*",
            CheckFileExists = true,
            Multiselect = false
        };

        bool? result = openFileDialog.ShowDialog();

        if (result == true)
        {
            ChatAvatarPath = openFileDialog.FileName;
            OnPropertyChanged(nameof(ChatAvatarPath));
        }
    }

    private void OnAddChats()
    {
        // Здесь можно добавить логику добавления чата
        RequestClose?.Invoke();
    }

    protected void OnPropertyChanged([CallerMemberName] string propName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; }

        public UserDto(int id, string username)
        {
            Id = id;
            Username = username;
        }
    }



}
