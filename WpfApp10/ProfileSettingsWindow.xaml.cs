using Microsoft.Win32;
using System.Windows;
using System.Windows.Media.Imaging;

namespace BroMessenger
{
    public partial class ProfileSettingsWindow : Window
    {
        public ProfileSettingsWindow()
        {
            InitializeComponent();

            // Загрузка текущих данных профиля
            NicknameTextBox.Text = "Текущий никнейм"; // Здесь нужно загружать реальные данные
            BirthDatePicker.SelectedDate = System.DateTime.Now.AddYears(-20); // Пример даты
        }

        private void ChangeAvatarButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Изображения (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|Все файлы (*.*)|*.*",
                Title = "Выберите изображение для аватара"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new System.Uri(openFileDialog.FileName);
                bitmap.EndInit();
                AvatarImage.Source = bitmap;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Здесь должна быть логика сохранения данных
            MessageBox.Show("Изменения сохранены", "Настройки профиля",
                           MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }
    }
}