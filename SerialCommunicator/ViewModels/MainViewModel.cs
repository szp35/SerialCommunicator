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
        public ICommand MinimizeWindowCommand { get; set; }
        public ICommand MaximizeRestoreCommand { get; set; }
        public ICommand CloseWindowCommand { get; set; }
        public ICommand RefreshCOMPortsCommand { get; set; }
        public ICommand ResetSerialViewCommand { get; set; }

        private ObservableCollection<string> _avaliableComPorts = new ObservableCollection<string>();
        public ObservableCollection<string> AvaliableCOMPorts
        {
            get => _avaliableComPorts;
            set => RaisePropertyChanged(ref _avaliableComPorts, value);
        }


        //private SerialViewModel _serialView;
        //public SerialViewModel SerialView
        //{
        //    get => _serialView;
        //    set => RaisePropertyChanged(ref _serialView, value);
        //}

        public SerialsListViewModel SerialsList { get; set; }
        public HelpViewModel Help { get; set; }
        public GraphWindow GraphWindow { get; set; }

        public MainViewModel()
        {
            MinimizeWindowCommand = new Command(Minimize);
            MaximizeRestoreCommand = new Command(MaximizeRestore);
            CloseWindowCommand = new Command(CloseWindow);
            RefreshCOMPortsCommand = new Command(RefreshCOMPorts);
            ResetSerialViewCommand = new Command(ResetSerialView);

            SerialsList = new SerialsListViewModel();
            SerialsList.MessageReceivedCallback = SerialMessageReceived;
            Help = new HelpViewModel();
            GraphWindow = new GraphWindow();
            //SerialsList.ItemChanged = SelectedSerialItemChanged;
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

        //public void SelectedSerialItemChanged(SerialItem item)
        //{
        //    if (item != null)
        //    {
        //        SerialView = item.SerialView;
        //    }
        //    else
        //        SerialView = null;
        //}

        public void SerialMessageReceived(string message)
        {
            int sizeCounter = 0;
            foreach(char letter in message)
            {
                sizeCounter += CharAlphabeticalPositions.CharToAlphabeticalPosition(letter);
            }

            Application.Current.Dispatcher.Invoke(() => { GraphWindow.GraphView.PlotGraph(Convert.ToDouble(sizeCounter)); });
        }

        public void ResetSerialView()
        {
            if (SerialsList.SelectedItem != null)
            {
                SerialsList.SelectedItem.SerialView.RestartSerialPort();
                SerialsList.SelectedItem.SerialView = new SerialViewModel();
            }
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
