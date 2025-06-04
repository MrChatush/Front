using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace WpfApp10
{
    public class AuthViewModel : INotifyPropertyChanged
    {
        private string _username = "Имя пользователя";
        private string _password = string.Empty;
        private bool _isUsernameFocused;
        private bool _isPasswordFocused;

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
            LoginCommand = new RelayCommand(_ => OnLogin(), _ => CanLogin());

            // Можно инициализировать Username и Password, если нужно
        }

        private bool CanLogin()
        {
            // Проверяем, что введено имя пользователя и пароль
            return !string.IsNullOrWhiteSpace(Username) && Username != "Имя пользователя"
                   && !string.IsNullOrEmpty(Password);
        }

        private void OnLogin()
        {
            // Здесь можно добавить логику авторизации
            // Если успешна, закрываем окно и открываем главное

            RequestClose?.Invoke();
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
