using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace WpfApp10
{
    public class RegistrationWindowViewModel : INotifyPropertyChanged
    {
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

        public RegistrationWindowViewModel()
        {
            RegisterCommand = new RelayCommand(_ => Register(), _ => CanRegister());
            ChangeAvatarCommand = new RelayCommand(_ => ChangeAvatar());
        }

        private bool CanRegister() =>
            !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password);

        private void Register()
        {
            // Здесь логика регистрации пользователя (например, HTTP-запрос)
            // После успешной регистрации:
            RequestClose?.Invoke();
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
