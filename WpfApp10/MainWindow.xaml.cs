using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
using System;

namespace BroMessenger
{


    public class Avatar
    {
        public ImageSource ImageSource { get; set; }
        public double Size { get; set; } = 30;
        public double CornerRadius { get; set; } = 20;
    }

    public class Message
    {
        public string Text { get; set; }
        public Avatar SenderAvatar { get; set; }
        public HorizontalAlignment Alignment { get; set; }
        public Brush MessageBackground => Alignment == HorizontalAlignment.Left ?
            Application.Current.Resources["MessageBackgroundBrush"] as Brush :
            new SolidColorBrush(Color.FromRgb(30, 109, 217));
    }

    public class ChatItem
    {
        public string Name { get; set; }
        public string LastMessage { get; set; }
        public string Time { get; set; }
        public bool IsOnline { get; set; }
        public string StatusColor => IsOnline ? "#FF4CAF50" : "#FF9E9E9E";
        public Avatar Avatar { get; set; }
    }

    public partial class MainWindow : Window
    {
        public ObservableCollection<ChatItem> Chats { get; set; }
        public ObservableCollection<Message> Messages { get; set; }
        public Avatar CurrentChatAvatar { get; set; }
        public string CurrentChatName { get; set; } = "Сережа";

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            // Загрузка аватарок 
            var avatar1 = new Avatar
            {
                ImageSource = new BitmapImage(new Uri("pack://application:,,,/Resources/Avatar2.png", UriKind.Absolute))
            };
            var avatar2 = new Avatar
            {
                ImageSource = new BitmapImage(new Uri("pack://application:,,,/Resources/Avatar2.png", UriKind.Absolute))
            };
            var avatar3 = new Avatar
            {
                ImageSource = new BitmapImage(new Uri("pack://application:,,,/Resources/Avatar2.png", UriKind.Absolute))
            };
            var avatar4 = new Avatar
            {
                ImageSource = new BitmapImage(new Uri("pack://application:,,,/Resources/Avatar2.png", UriKind.Absolute))
            };
            var avatar5 = new Avatar
            {
                ImageSource = new BitmapImage(new Uri("pack://application:,,,/Resources/Avatar2.png", UriKind.Absolute))
            };

            // Инициализация чатов
            Chats = new ObservableCollection<ChatItem>
            {
                new ChatItem { Name = "Максим", LastMessage = "Привет, как дела?", Time = "12:30", IsOnline = false, Avatar = avatar1 },
                new ChatItem { Name = "Сережа", LastMessage = "Давай встретимся", Time = "10:15", IsOnline = true, Avatar = avatar2 },
                new ChatItem { Name = "Саня", LastMessage = "Не забудь купить пиво", Time = "Вчера", IsOnline = true, Avatar = avatar3 },
                new ChatItem { Name = "Семён", LastMessage = "Посмотри это видео", Time = "Пн", IsOnline = false, Avatar = avatar4 },
                new ChatItem { Name = "КУКУРУЗА", LastMessage = "По работе вопрос", Time = "21.05", IsOnline = true, Avatar = avatar5 }
            };

            ChatsList.ItemsSource = Chats;
            CurrentChatAvatar = avatar1;

            // Инициализация сообщений
            Messages = new ObservableCollection<Message>
            {
                new Message { Text = "Ты как", SenderAvatar = avatar2, Alignment = HorizontalAlignment.Left },
                new Message { Text = "Норм", SenderAvatar = avatar1, Alignment = HorizontalAlignment.Right },
                new Message { Text = "Давай встретимся", SenderAvatar = avatar2, Alignment = HorizontalAlignment.Left }
            };
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.Owner = this; // Устанавливаем главное окно как владельца
            settingsWindow.ShowDialog(); // Показываем как модальное окно
        }
    }
}