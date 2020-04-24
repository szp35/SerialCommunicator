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
    /// Interaction logic for SerialMessage.xaml
    /// </summary>
    public partial class SerialMessage : UserControl
    {
        public SerialMessage()
        {
            InitializeComponent();
        }
        public SerialMessage(string rxORtx, string message, DateTime time)
        {
            InitializeComponent();
            SerialMessageModel model = new SerialMessageModel();
            model.RXorTX = rxORtx;
            model.Message = message;
            model.Time = time;
            DataContext = model;
        }

        public SerialMessage(SerialMessageModel messageVM, int index)
        {
            InitializeComponent();
            messageVM.CountIndex = index.ToString();
            DataContext = messageVM;
        }
        public SerialMessage(SerialMessageModel messageVM)
        {
            InitializeComponent();
            DataContext = messageVM;
        }
    }
}
