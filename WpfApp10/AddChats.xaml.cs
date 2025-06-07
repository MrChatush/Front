using Microsoft.AspNetCore.SignalR.Client;
using System.Net.Http;
using System.Windows;

namespace WpfApp10
{
    public partial class AddChats : Window
    {
        public AddChats(HubConnection hubConnection, HttpClient httpClient, string token)
        {
            InitializeComponent();
            var vm = new AddChatsViewModel(hubConnection, httpClient, token);
            DataContext = vm;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
