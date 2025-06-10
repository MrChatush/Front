using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.AspNetCore.SignalR.Client;

namespace WpfApp10
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow(HubConnection hubConnection, HttpClient httpClient, string token,int ChatId, Func<Task> Update)
        {
            InitializeComponent();
            var vm = new SettingsViewModel(hubConnection, httpClient, token,ChatId,Update);
            vm.CloseWindowAction = () => this.Close();
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
