using System;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace WpfApp10
{
    public class RegistrationWindowViewModel : INotifyPropertyChanged
    {
        private readonly ApiAuthService _authService;

        private string _username;
        private string _password;
        private BitmapImage _avatar;
        public event PropertyChangedEventHandler PropertyChanged;
        public event Action RequestClose;

        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); }
        }

        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        public BitmapImage Avatar
        {
            get => _avatar;
            set { _avatar = value; OnPropertyChanged(); }
        }

        public ICommand RegisterCommand { get; }
        public ICommand ChangeAvatarCommand { get; }

        public RegistrationWindowViewModel(ApiAuthService authService)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            RegisterCommand = new RelayCommand(async _ => await Register(), _ => CanRegister());
            ChangeAvatarCommand = new RelayCommand(_ => ChangeAvatar());
        }


        private bool CanRegister() =>
            !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password);

        private async Task Register()
        {
            //try
            //{
            //    var response = await _authService.RegisterAsync(Username, Password);
            //    var loginResult = await _authService.LoginAsync(Username, Password);

            //    // Сохранение токена
            //    Application.Current.Properties["JwtToken"] = loginResult.Token;
            //    RequestClose?.Invoke();
            //}
            //catch (HttpRequestException ex)
            //{
            //    MessageBox.Show("Ошибка входа: " + ex);
            //}

            var client = new HttpClient();
            var loginData = new { Username = this.Username, Password = this.Password };

            try
            {
                var response = await client.PostAsJsonAsync("https://localhost:7000/api/auth/register", loginData);

                if (response.IsSuccessStatusCode)
                {
                    //var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                    //Token = result.token;
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

        private void ChangeAvatar()
        {
            // Открытие диалога выбора файла и установка аватара
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpg)|*.png;*.jpg"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                Avatar = new BitmapImage(new Uri(openFileDialog.FileName));
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }



}
