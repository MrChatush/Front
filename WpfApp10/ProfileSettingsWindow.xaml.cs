using System.Windows;

namespace WpfApp10
{
    public partial class ProfileSettingsWindow : Window
    {
        public ProfileSettingsWindow()
        {
            InitializeComponent();
            var vm = new ProfileSettingsViewModel();
            vm.CloseAction = () => this.Close();
            DataContext = vm;
        }
    }
}
