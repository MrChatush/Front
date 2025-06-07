using WpfApp10;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp10
{
    public partial class AuthWindow : Window
    {
        private readonly AuthViewModel _viewModel;

        public AuthWindow()
        {
            InitializeComponent();
            _viewModel = new AuthViewModel();
            _viewModel.RequestClose += OnRequestClose;
            _viewModel.RequestRegister += OnRequestRegister; // добавьте эту строку
            
            DataContext = _viewModel;
        }

        private void OnRequestRegister()
        {
            var regWindow = new RegistrationWindow();
            regWindow.Show();
            this.Close();
        }

        private void OnRequestClose()
        {
            MainWindow mainWindow = new MainWindow(_viewModel.Token);
            mainWindow.Show();
            this.Close();
        }

        private void UsernameBox_GotFocus(object sender, RoutedEventArgs e)
        {
            _viewModel.UsernameGotFocus();
        }

        private void UsernameBox_LostFocus(object sender, RoutedEventArgs e)
        {
            _viewModel.UsernameLostFocus();
        }

        private void PasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            _viewModel.PasswordGotFocus();
        }

        private void PasswordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            _viewModel.PasswordLostFocus();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            if (passwordBox != null)
            {
                _viewModel.Password = passwordBox.Password;
            }
        }
    }
}
