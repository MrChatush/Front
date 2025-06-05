using System;
using System.ComponentModel;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Net.Http.Json;
namespace WpfApp10
{
    public class AuthViewModel : INotifyPropertyChanged
    {
        public ICommand RegisterCommand { get; }
        public event Action RequestRegister;

        private string _username = "Имя пользователя";
        private string _password = string.Empty;
        private bool _isUsernameFocused;
        private bool _isPasswordFocused;
        private string _token;
        public string Token
        {
            get => _token;
            set { _token = value; OnPropertyChanged(); }
        }
        public event PropertyChangedEventHandler PropertyChanged;

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

        public string Password
        {
            get => _password;
            set
            {
                if (_password != value)
                {
                    _password = value;
                    OnPropertyChanged();
                }
            }
        }

        // Для управления placeholder-ами
        public bool IsUsernamePlaceholderVisible => string.IsNullOrWhiteSpace(Username) && !_isUsernameFocused;
        public bool IsPasswordPlaceholderVisible => string.IsNullOrEmpty(Password) && !_isPasswordFocused;

        public ICommand LoginCommand { get; }

        // Событие для закрытия окна
        public event Action RequestClose;


        public AuthViewModel()
        {
            LoginCommand = new RelayCommand(async _ => await OnLoginAsync(), _ => CanLogin());
            RegisterCommand = new RelayCommand(_ => OnRegister());
        }

        private void OnRegister()
        {
            RequestRegister?.Invoke();
        }

        private bool CanLogin()
        {
            // Проверяем, что введено имя пользователя и пароль
            return !string.IsNullOrWhiteSpace(Username) && Username != "Имя пользователя"
                   && !string.IsNullOrEmpty(Password);
        }

        private async Task OnLoginAsync()
        {
            var client = new HttpClient();
            var loginData = new { Username = this.Username, Password = this.Password };

            try
            {
                var response = await client.PostAsJsonAsync("https://localhost:7000/api/auth/login", loginData);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                    Token = result.token;
                    // Можно сохранить токен в сервис или статическое поле для дальнейших запросов
                    RequestClose?.Invoke();
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    MessageBox.Show("Ошибка входа: " + error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка соединения: " + ex.Message);
            }
        }

        private class LoginResponse
        {
            public string token { get; set; }
            public int userId { get; set; }
            public string username { get; set; }
        }


        // Методы для обработки фокуса в View через биндинг
        public void UsernameGotFocus()
        {
            _isUsernameFocused = true;
            if (Username == "Имя пользователя")
                Username = string.Empty;
            OnPropertyChanged(nameof(IsUsernamePlaceholderVisible));
        }

        public void UsernameLostFocus()
        {
            _isUsernameFocused = false;
            if (string.IsNullOrWhiteSpace(Username))
                Username = "Имя пользователя";
            OnPropertyChanged(nameof(IsUsernamePlaceholderVisible));
        }

        public void PasswordGotFocus()
        {
            _isPasswordFocused = true;
            OnPropertyChanged(nameof(IsPasswordPlaceholderVisible));
        }

        public void PasswordLostFocus()
        {
            _isPasswordFocused = false;
            OnPropertyChanged(nameof(IsPasswordPlaceholderVisible));
        }

        protected void OnPropertyChanged([CallerMemberName] string propName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }
}
