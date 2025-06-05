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
            _viewModel = new RegistrationWindowViewModel();
            _viewModel.RequestClose += OnRequestClose;
            DataContext = _viewModel;
        }

        private void OnRequestClose()
        {
            var mainWindow = new MainWindow();
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
