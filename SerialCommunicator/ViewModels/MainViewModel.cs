using SerialCommunicator.Controls;
using SerialCommunicator.Utilities;
using SerialCommunicator.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace SerialCommunicator.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public ICommand RefreshCOMPortsCommand { get; set; }
        public ICommand ResetSerialViewCommand { get; set; }
        public ObservableCollection<string> AvaliableCOMPorts { get; set; }
        public SerialsListViewModel SerialsList { get; set; }
        public HelpViewModel Help { get; set; }

        public MainViewModel()
        {
            AvaliableCOMPorts = new ObservableCollection<string>();
            RefreshCOMPortsCommand = new Command(RefreshCOMPorts);
            ResetSerialViewCommand = new Command(ResetSerialView);

            SerialsList = new SerialsListViewModel();
            Help = new HelpViewModel();
            RefreshCOMPorts();
        }

        public void RefreshCOMPorts()
        {
            AvaliableCOMPorts.Clear();
            foreach(string port in SerialPort.GetPortNames())
            {
                AvaliableCOMPorts.Add(port);
            }
        }

        public void ResetSerialView()
        {
            if (SerialsList.SelectedItem != null)
            {
                SerialsList.SelectedItem.ItemView.SerialView.ShutdownEverything();
                string oldName = SerialsList.SelectedItem.ItemView.SerialView.SerialItemName;
                SerialsList.SelectedItem.ItemView.SerialView = new SerialViewModel(oldName);
            }
        }
    }
}
