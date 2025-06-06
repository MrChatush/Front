using System;
using System.Windows;

namespace WpfApp10
{
    public partial class MainWindow : Window
    {
        private MainWindowViewModel ViewModel => DataContext as MainWindowViewModel;

        public MainWindow(string Token)
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel(Token);

            Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //if (ViewModel != null)
            //{
            //    // Загружаем чаты при загрузке окна
            //    try
            //    {
            //        await ViewModel.LoadChatsAsync();
            //        await ViewModel.ReconnectSignalRAsync();
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show(ex.Message);
            //    }

            //    // Инициализируем подключение к SignalR
                
            //}
            //else
            //{
            //    MessageBox.Show("Анлак");
            //}
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            // Вызываем команду добавления чата из VM
            ViewModel?.AddChatCommand.Execute(null);

            // После добавления чата можно обновить список, если команда не делает этого автоматически
            // await ViewModel.LoadChatsAsync(); // если нужно, можно раскомментировать
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            // Вызываем команду открытия настроек из VM
            ViewModel?.OpenSettingsCommand.Execute(null);
        }
    }
}
