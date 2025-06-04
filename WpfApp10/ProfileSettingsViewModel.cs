using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace WpfApp10
{
    public class ProfileSettingsViewModel : INotifyPropertyChanged
    {
        private string _nickname = "Текущий никнейм";
        private DateTime? _birthDate = DateTime.Now.AddYears(-20);
        private BitmapImage _avatarImage;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Nickname
        {
            get => _nickname;
            set
            {
                if (_nickname != value)
                {
                    _nickname = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime? BirthDate
        {
            get => _birthDate;
            set
            {
                if (_birthDate != value)
                {
                    _birthDate = value;
                    OnPropertyChanged();
                }
            }
        }

        public BitmapImage AvatarImage
        {
            get => _avatarImage;
            set
            {
                if (_avatarImage != value)
                {
                    _avatarImage = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand ChangeAvatarCommand { get; }
        public ICommand SaveCommand { get; }
        public Action CloseAction { get; set; }

        public ProfileSettingsViewModel()
        {
            ChangeAvatarCommand = new RelayCommand(_ => ChangeAvatar());
            SaveCommand = new RelayCommand(_ => Save());
            // Инициализация аватара (если есть)
            // Можно загрузить из настроек или файла
        }

        private void ChangeAvatar()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Изображения (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|Все файлы (*.*)|*.*",
                Title = "Выберите изображение для аватара"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(openFileDialog.FileName);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    AvatarImage = bitmap;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки изображения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Save()
        {
            // Здесь логика сохранения данных, например, в настройки или базу

            MessageBox.Show("Изменения сохранены", "Настройки профиля", MessageBoxButton.OK, MessageBoxImage.Information);

            CloseAction?.Invoke();
        }

        protected void OnPropertyChanged([CallerMemberName] string propName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }
}
