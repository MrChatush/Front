using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.IO;
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
        private string _avatarFilePath;
        public event PropertyChangedEventHandler PropertyChanged;
        public event Action RequestClose;
        private string _token;
        private object AvatarFile;
        private BitmapImage _avatar;

        public string AvatarFilePath
        {
            get => _avatarFilePath;
            set { _avatarFilePath = value; OnPropertyChanged(); }
        }
        public string Token
        {
            get => _token;
            set { _token = value; OnPropertyChanged(); }
        }
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

        public event Action<string> TokenReceived;
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
            var client = new HttpClient();

            try
            {
                var content = new MultipartFormDataContent();

;

                if (!string.IsNullOrEmpty(AvatarFilePath) && File.Exists(AvatarFilePath))
                {
                    var fileStream = File.OpenRead(AvatarFilePath);
                    var fileContent = new StreamContent(fileStream);
                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg"); // или определите тип по расширению
                    content.Add(fileContent, "Avatar", Path.GetFileName(AvatarFilePath));
                }
                content.Add(new StringContent(this.Username), "Username");
                content.Add(new StringContent(this.Password), "Password");
                var response = await client.PostAsync("https://localhost:7000/api/auth/register", content);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<LoginReponse>();
                    Token = result.token;
                    TokenReceived?.Invoke(Token);
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

        private class LoginReponse
        {
            public string token { get; set; }
            public int userId { get; set; }
            public string username { get; set; }
        }
        private void ChangeAvatar()
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpg)|*.png;*.jpg"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                AvatarFilePath = openFileDialog.FileName;
                Avatar = new BitmapImage(new Uri(AvatarFilePath));
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }



}
