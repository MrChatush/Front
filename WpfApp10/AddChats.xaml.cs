using System.Windows;

namespace WpfApp10
{
    public partial class AddChats : Window
    {
        public AddChats()
        {
            InitializeComponent();
            var vm = new AddChatsViewModel();
            vm.RequestClose += () => this.Close();
            DataContext = vm;
        }
    }
}
