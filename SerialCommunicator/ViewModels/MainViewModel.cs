using SerialCommunicator.Controls;
using SerialCommunicator.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SerialCommunicator.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public ICommand MinimizeWindowCommand { get; set; }
        public ICommand MaximizeRestoreCommand { get; set; }
        public ICommand CloseWindowCommand { get; set; }
        public ICommand RefreshCOMPortsCommand { get; set; }

        private ObservableCollection<string> _avaliableComPorts = new ObservableCollection<string>();
        public ObservableCollection<string> AvaliableCOMPorts
        {
            get => _avaliableComPorts;
            set => RaisePropertyChanged(ref _avaliableComPorts, value);
        }

        public SerialsListViewModel SerialsList { get; set; }

        private SerialViewModel _serialView;
        public SerialViewModel SerialView
        {
            get => _serialView;
            set => RaisePropertyChanged(ref _serialView, value);
        }

        public MainViewModel()
        {
            MinimizeWindowCommand = new Command(Minimize);
            MaximizeRestoreCommand = new Command(MaximizeRestore);
            CloseWindowCommand = new Command(CloseWindow);
            RefreshCOMPortsCommand = new Command(RefreshCOMPorts);

            SerialsList = new SerialsListViewModel();
            SerialsList.ItemChanged = SelectedSerialItemChanged;
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

        public void SelectedSerialItemChanged(SerialItem item)
        {
            if (item != null)
            {
                SerialView = item.SerialView;
            }
            else
                SerialView = null;
        }

        #region Window Methods

        public void CloseWindow() => Application.Current.MainWindow.Close();
        public void MaximizeRestore()
        {
            Window mainWindow = Application.Current.MainWindow;
            if (mainWindow.WindowState == WindowState.Maximized)
                mainWindow.WindowState = WindowState.Normal;
            else if (mainWindow.WindowState == WindowState.Normal)
                mainWindow.WindowState = WindowState.Maximized;
        }
        public void Minimize() => Application.Current.MainWindow.WindowState = WindowState.Minimized;

        #endregion
    }
}
