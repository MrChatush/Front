using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

public class AddChatsViewModel : INotifyPropertyChanged
{
    private string _username;
    public event PropertyChangedEventHandler PropertyChanged;

    public string Username
    {
        get => _username;
        set
        {
            if (_username != value)
            {
                _username = value;
                OnPropertyChanged();
            }
        }
    }

    public ICommand AddChatsCommand { get; }

    // Событие для закрытия окна
    public event Action RequestClose;

    public AddChatsViewModel()
    {
        AddChatsCommand = new RelayCommand(_ => OnAddChats());
    }

    private void OnAddChats()
    {
        // Здесь можно добавить логику добавления чата
        RequestClose?.Invoke();
    }

    protected void OnPropertyChanged([CallerMemberName] string propName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
}
