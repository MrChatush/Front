using BroMessenger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApp10
{
    /// <summary>
    /// Логика взаимодействия для AddChats.xaml
    /// </summary>
    public partial class AddChats : Window
    {
        public AddChats()
        {
            InitializeComponent();
        }
        private void AddChats_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        } 
    }
}
