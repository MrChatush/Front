using System.Windows;

namespace WpfApp10
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            var vm = new SettingsViewModel();
            vm.CloseWindowAction = () => this.Close();
            vm.OpenProfileSettingsAction = () =>
            {
                var profileWindow = new ProfileSettingsWindow();
                profileWindow.Owner = this;
                profileWindow.ShowDialog();
            };
            vm.OpenAuthWindowAction = () =>
            {
                var authWindow = new AuthWindow();
                authWindow.Show();
                // Закрытие всех других окон, кроме authWindow
                foreach (Window window in Application.Current.Windows)
                {
                    if (window != authWindow)
                        window.Close();
                }
            };
            DataContext = vm;
        }
    }
}
