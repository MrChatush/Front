using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.AspNetCore.SignalR.Client;

namespace WpfApp10
{
    public partial class MainWindow : Window
    {
        private SettingsViewModel _settingsViewModel;

        private MainWindowViewModel ViewModel => DataContext as MainWindowViewModel;

        public MainWindow(string Token)
        {
            InitializeComponent();

            DataContext = new MainWindowViewModel(Token);

            // Подписка на событие очистки истории
            Loaded += MainWindow_Loaded;
            ((INotifyCollectionChanged)ViewModel.Messages).CollectionChanged += Messages_CollectionChanged;
            this.Closing += MainWindow_Closing;
        }

        private async void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            // Вызвать метод обновления статуса пользователя на сервере
            try
            {

            }
            catch (Exception ex)
            {
                // Логирование ошибки, если нужно
            }
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

        private void Messages_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //if (VisualTreeHelper.GetChild(MessagesControl, 0) is Border border &&
            //    VisualTreeHelper.GetChild(border, 0) is ScrollViewer scrollViewer)
            //{
            //    scrollViewer.ScrollToEnd();
            //}
        }

    }
}
