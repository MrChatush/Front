using System.Windows;
using System.Windows.Controls;

namespace WpfApp10
{
    public partial class RegistrationWindow : Window
    {
        private readonly RegistrationWindowViewModel _viewModel;

        public RegistrationWindow()
        {
            InitializeComponent();
            ApiAuthService api = new ApiAuthService();
            _viewModel = new RegistrationWindowViewModel(api);
            _viewModel.RequestClose += OnRequestClose;
            DataContext = _viewModel;
        }

        private void OnRequestClose()
        {
            var mainWindow = new MainWindow(_viewModel.Token);
            mainWindow.Show();
            this.Close();
        }
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is RegistrationWindowViewModel vm)
                vm.Password = ((PasswordBox)sender).Password;
        }
    }
}
