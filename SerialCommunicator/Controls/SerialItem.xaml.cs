using SerialCommunicator.ViewModels;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SerialCommunicator.Controls
{
    /// <summary>
    /// Interaction logic for SerialItem.xaml
    /// </summary>
    public partial class SerialItem : UserControl
    {
        public Action<SerialItem> Close { get; set; }
        public SerialViewModel SerialView { get; set; }

        public SerialItem(string name)
        {
            InitializeComponent();
            SerialView = new SerialViewModel(name);
            DataContext = SerialView;
            SerialView.SerialItemName = name;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close?.Invoke(this);
        }
    }
}
