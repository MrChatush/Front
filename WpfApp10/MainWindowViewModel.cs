using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;


namespace WpfApp10
{
    public class MainWindowViewModel : ObservableObject // ObservableObject реализует INotifyPropertyChanged (можно реализовать вручную)
    {
        //public ObservableCollection<ChatItem> Chats { get; set; }
        //public ObservableCollection<Message> Messages { get; set; }

        //private Avatar _currentChatAvatar;
        //public Avatar CurrentChatAvatar
        //{
        //    get => _currentChatAvatar;
        //    set { _currentChatAvatar = value; OnPropertyChanged(); }
        //}

        private string _currentChatName = "Сережа";
        public string CurrentChatName
        {
            get => _currentChatName;
            set { _currentChatName = value; OnPropertyChanged(); }
        }

        private string _messageText = "";
        public string MessageText
        {
            get => _messageText;
            set { _messageText = value; OnPropertyChanged(); }
        }

        public ICommand OpenSettingsCommand { get; }
        public ICommand AddChatCommand { get; }
        public ICommand SendMessageCommand { get; }

        public MainWindowViewModel()
        {
            // Загрузка аватарок
            //var avatar1 = new Avatar { ImageSource = new BitmapImage(new Uri("pack://application:,,,/Resources/Avatar2.png")) };
            //var avatar2 = new Avatar { ImageSource = new BitmapImage(new Uri("pack://application:,,,/Resources/Avatar2.png")) };
            //var avatar3 = new Avatar { ImageSource = new BitmapImage(new Uri("pack://application:,,,/Resources/Avatar2.png")) };
            //var avatar4 = new Avatar { ImageSource = new BitmapImage(new Uri("pack://application:,,,/Resources/Avatar2.png")) };
            //var avatar5 = new Avatar { ImageSource = new BitmapImage(new Uri("pack://application:,,,/Resources/Avatar2.png")) };

            // Инициализация чатов
            //Chats = new ObservableCollection<ChatItem>
            //{
            //    new ChatItem { Name = "Максим", LastMessage = "Привет, как дела?", Time = "12:30", IsOnline = false, Avatar = avatar1 },
            //    new ChatItem { Name = "Сережа", LastMessage = "Давай встретимся", Time = "10:15", IsOnline = true, Avatar = avatar2 },
            //    new ChatItem { Name = "Саня", LastMessage = "Не забудь купить пиво", Time = "Вчера", IsOnline = true, Avatar = avatar3 },
            //    new ChatItem { Name = "Семён", LastMessage = "Посмотри это видео", Time = "Пн", IsOnline = false, Avatar = avatar4 },
            //    new ChatItem { Name = "КУКУРУЗА", LastMessage = "По работе вопрос", Time = "21.05", IsOnline = true, Avatar = avatar5 }
            //};

            //CurrentChatAvatar = avatar1;

            // Инициализация сообщений
            //Messages = new ObservableCollection<Message>
            //{
            //    new Message { Text = "Ты как", SenderAvatar = avatar2, Alignment = HorizontalAlignment.Left },
            //    new Message { Text = "Норм", SenderAvatar = avatar1, Alignment = HorizontalAlignment.Right },
            //    new Message { Text = "Давай встретимся", SenderAvatar = avatar2, Alignment = HorizontalAlignment.Left }
            //};

            // Команды
            OpenSettingsCommand = new RelayCommand(_ => OpenSettings());
            AddChatCommand = new RelayCommand(_ => AddChat());
            SendMessageCommand = new RelayCommand(_ => SendMessage(), _ => !string.IsNullOrWhiteSpace(MessageText));
        }

        private void OpenSettings()
        {
            var win = new SettingsWindow();
            win.ShowDialog();
        }

        private void AddChat()
        {
            var win = new AddChats();
            win.ShowDialog();
        }

        private void SendMessage()
        {
            //Messages.Add(new Message
            //{
            //    Text = MessageText,
            //    SenderAvatar = CurrentChatAvatar,
            //    Alignment = HorizontalAlignment.Right
            //});
            //MessageText = "";
        }
    }
}
