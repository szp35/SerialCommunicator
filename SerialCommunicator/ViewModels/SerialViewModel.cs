using SerialCommunicator.Controls;
using SerialCommunicator.Utilities;
using System;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SerialCommunicator.ViewModels
{
    public class SerialViewModel : BaseViewModel
    {
        #region Commands
        public ICommand ConnectDisconnedCommand { get; set; }
        public ICommand ClearReceivedMessagesCommand { get; set; }
        public ICommand ClearSentMessagesCommand { get; set; }
        public ICommand ClearBuffersCommand { get; set; }
        public ICommand SendMessageCommand { get; set; }
        public ICommand ResetSerialPortCommand { get; set; }
        #endregion

        #region Private Fields

        private string _serialItemName;
        private bool _isConnected;

        private double _sendTimeout;
        private double _receiveTimeout;
        private double _bufferSize;
        private double _maxReceivableMessages;

        private string _comName;
        private string _activeComName;
        private string _baudRate;
        private string _dataBits;
        private string _stopBits;
        private string _parity;
        private string _handShake;
        private bool _dataTerminalReady;
        private string _cnctDcnctBtnContent;

        private ObservableCollection<SerialMessage> _receivedMessages = new ObservableCollection<SerialMessage>();
        private ObservableCollection<SerialMessage> _sentMessages = new ObservableCollection<SerialMessage>();

        private string _toBeSentText;
        private bool _waitingStatus;

        #endregion

        #region Public Fields
        public string SerialItemName
        {
            get => _serialItemName;
            set => RaisePropertyChanged(ref _serialItemName, value);
        }
        public bool IsConnected
        {
            get => _isConnected;
            set => RaisePropertyChanged(ref _isConnected, value);
        }
        public double SendTimeout
        {
            get => _sendTimeout;
            set { RaisePropertyChanged(ref _sendTimeout, value); UpdateSendTimeout(int.Parse(Math.Round(value, 0).ToString())); }
        }
        public double ReceiveTimeout
        {
            get => _receiveTimeout;
            set { RaisePropertyChanged(ref _receiveTimeout, value); UpdateReceiveTimeout(int.Parse(Math.Round(value, 0).ToString())); }
        }
        public double BufferSize
        {
            get => _bufferSize;
            set { RaisePropertyChanged(ref _bufferSize, value); UpdateBufferSize(int.Parse(Math.Round(value, 0).ToString())); }
        }
        public double MaxReceivableMessages
        {
            get => _maxReceivableMessages;
            set { RaisePropertyChanged(ref _maxReceivableMessages, value); UpdateMaxReceivableMessages(int.Parse(Math.Round(value, 0).ToString())); }
        }
        public string COMName
        {
            get => _comName;
            set { RaisePropertyChanged(ref _comName, value); UpdateCOMName(value); }
        }
        public string ActiveCOMName
        {
            get => _activeComName;
            set => RaisePropertyChanged(ref _activeComName, value);
        }
        public string BaudRate
        {
            get => _baudRate;
            set { RaisePropertyChanged(ref _baudRate, value); UpdateBaudRate(int.Parse(value)); }
        }
        public string DataBits
        {
            get => _dataBits;
            set { RaisePropertyChanged(ref _dataBits, value); UpdateDataBits(int.Parse(value)); }
        }
        public string StopBits
        {
            get => _stopBits;
            set { RaisePropertyChanged(ref _stopBits, value); UpdateStopBits(value); }
        }
        public string Parity
        {
            get => _parity;
            set { RaisePropertyChanged(ref _parity, value); UpdateParity(value); }
        }
        public string HandShake
        {
            get => _handShake;
            set { RaisePropertyChanged(ref _handShake, value); UpdateHandShake(value); }
        }
        public bool DataTerminalReady
        {
            get => _dataTerminalReady;
            set { RaisePropertyChanged(ref _dataTerminalReady, value); UpdateDTR(value); }
        }
        public string ConnectDisconnectButtonContent
        {
            get => _cnctDcnctBtnContent;
            set => RaisePropertyChanged(ref _cnctDcnctBtnContent, value);
        }
        public ObservableCollection<SerialMessage> ReceivedMessages
        {
            get => _receivedMessages;
            set => RaisePropertyChanged(ref _receivedMessages, value);
        }
        public ObservableCollection<SerialMessage> SentMessages
        {
            get => _sentMessages;
            set => RaisePropertyChanged(ref _sentMessages, value);
        }
        public string ToBeSentText
        {
            get => _toBeSentText;
            set => RaisePropertyChanged(ref _toBeSentText, value);
        }
        public bool WaitingStatus
        {
            get => _waitingStatus;
            set => RaisePropertyChanged(ref _waitingStatus, value);
        }
        #endregion

        public string ReceivedDataBuffer { get; set; }

        private TransceiveSettingsViewModel _settings = new TransceiveSettingsViewModel();
        public TransceiveSettingsViewModel Settings
        {
            get => _settings;
            set => RaisePropertyChanged(ref _settings, value);
        }
        public SerialPort SerialPort { get; set; }

        #region Constructor

        public SerialViewModel()
        {
            ConnectDisconnedCommand = new Command(AutoConnectDisconnect);
            SendMessageCommand = new Command(SendMessage);
            ClearReceivedMessagesCommand = new Command(ClearReceivedMessages);
            ClearSentMessagesCommand = new Command(ClearSentMessages);
            ClearBuffersCommand = new Command(ClearBuffers);
            ResetSerialPortCommand = new Command(RestartSerialPort);

            RestartSerialPort();
            Settings = new TransceiveSettingsViewModel();

            // 0.5 seconds, is relatively reasonable.
            SendTimeout = 500;
            ReceiveTimeout = 500;
            BufferSize = 4096;
            // too many could slow down the program
            MaxReceivableMessages = 150;
            COMName = "COM1";
            ActiveCOMName = COMName;
            BaudRate = "9600";
            DataBits = "8";
            StopBits = "One";
            Parity = "None";
            HandShake = "None";
            ConnectDisconnectButtonContent = "Connect";

            UpdateSerialValues();
        }

        #endregion

        #region Crucial SerialPort methods

        public void AutoConnectDisconnect()
        {
            if (IsConnected)
                Disconnect();
            else
                Connect();
        }

        public void Connect()
        {
            if (ActiveCOMName == "COM1")
            {
                AlertMessage("Cannot use COM1 because it crashes the program");
                return;
            }
            try
            {
                UpdateSerialValues();
                SetWaitingStatus(true);
                SerialPort.Open();
                IsConnected = true;
                SetWaitingStatus(false);
                AlertMessage($"Successfully connected to {ActiveCOMName}.");
                ConnectDisconnectButtonContent = "Disconnect";
                WriteExtraBufferData();
            }
            catch (Exception e)
            {
                ErrorMessage($"Failed to connect to port: {e.Message}");
            }
        }

        public void Disconnect()
        {
            try
            {
                SetWaitingStatus(true);
                Task.Run(() =>
                {
                    SerialPort.Close();
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        SetWaitingStatus(false);
                        AlertMessage($"Successfully disconnected from {ActiveCOMName}.");
                        IsConnected = false;
                        ConnectDisconnectButtonContent = "Connect";
                    });
                });
            }
            catch (Exception e)
            {
                ErrorMessage($"Failed to disconnect from port: {e.Message}");
            }
        }

        public void WriteExtraBufferData()
        {
            if (SerialPort != null && SerialPort.IsOpen && SerialPort.BytesToRead > 0)
            {
                BufferMessage(SerialPort.ReadExisting());
            }
        }

        public void RestartSerialPort()
        {
            if (SerialPort != null)
                SerialPort.Dispose();
            SerialPort = new SerialPort();
            SerialPort.DataReceived += SerialPort_DataReceived;
            SerialPort.ErrorReceived += SerialPort_ErrorReceived;
        }

        #endregion

        #region SerialPort Events

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Task.Run(() =>
            {
                if (SerialPort.IsOpen)
                {
                    try
                    {
                        if (Settings.ReceiveWithNewLine)
                        {
                            MessageReceived(ReadLine());
                            //MessageReceived(SerialPort.ReadLine());
                        }
                        else if (Settings.ReceiveWithNothingElse)
                        {
                            MessageReceived(ReadRawText());
                        }
                        else if (Settings.ReceiveWithCustomTag)
                        {
                            MessageReceived(SerialPort.ReadTo(Settings.CustomTag));
                        }
                    }
                    catch (TimeoutException)
                    {
                        ErrorMessage($"Received message has timed out.");
                    }
                    catch (Exception ee)
                    {
                        ErrorMessage($"Error trying to parse received data: {ee.Message}");
                    }
                }
                else
                {
                    ErrorMessage($"SerialPort is not connected.");
                }
            });
        }


        private void SerialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            switch (e.EventType)
            {
                case SerialError.Frame:    
                    ErrorMessage(
                        "Framing error detected: attempted to read from wrong starting point of data. " +
                        "solution: [RESTART SERIALPORT]");
                    RestartSerialPort(); break;
                case SerialError.Overrun:  
                    ErrorMessage(
                        "Overrun error detected: data arrived before previous data could be processed. " +
                        "solution: [RESTART SERIALPORT.]");
                    RestartSerialPort(); break;
                case SerialError.RXOver:   
                    ErrorMessage(
                        "RXOver error detected: the receive buffer is full, or data was received after end-of-file marker. " +
                        "solution: [(user)CLEAR BUFFERS]"); break;
                case SerialError.RXParity:
                    ErrorMessage(
                        "RXParity error detected: parity might not have been applied, or data was corrupted. " +
                        "solution: [none]"); break;
                case SerialError.TXFull:   
                    ErrorMessage(
                        "TXFull error detected: attempted to transmit data when output buffer was full." +
                        " solution: [(user)CLEAR BUFFERS]"); break;
            }
        }

        #endregion

        #region GUI Messaging Methods

        //all thread safe
        public void BufferMessage(string message) => AddAutomaticMessage("Buffer", message);
        public void AlertMessage(string message) => AddAutomaticMessage("Alert", message);
        public void ErrorMessage(string errMessage) => AddAutomaticMessage("Error", errMessage);
        public void MessageReceived(string message) => AddAutomaticMessage("RX", message);
        public void MessageSent(string message) => AddAutomaticMessage("TX", message);
        //thread safe
        public void AddAutomaticMessage(string transmissionDirection, string message)
        {
            AddAutomaticMessage(new SerialMessageModel() { Message = message, RXorTX = transmissionDirection, Time = DateTime.Now });
        }
        //thread safe
        public void AddAutomaticMessage(SerialMessageModel message)
        {
            if (message.RXorTX == "TX")
                AddSentMessage(message);
            else
                AddReceivedMessage(message);
        }
        //thread safe
        public void AddReceivedMessage(SerialMessageModel message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                ReceivedMessages.Insert(0, new SerialMessage(message));
            });
        }
        //thread safe
        public void AddSentMessage(SerialMessageModel message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                SentMessages.Insert(0, new SerialMessage(message));
            });
        }

        #endregion

        #region SerialPort Transmit Methods

        public void SendMessage()
        {
            if (!string.IsNullOrEmpty(ToBeSentText))
                SendMessage(ToBeSentText);
        }

        public void SendMessage(string message)
        {
            if (SerialPort.IsOpen)
            {
                //async due to timeouts halting main thread
                Task.Run(() =>
                {
                    try
                    {
                        SetWaitingStatus(true);

                        if (Settings.SendWithNewLine)
                            SerialPort.WriteLine(message);
                        else if (Settings.SendWithCustomTag)
                            SerialPort.Write($"{message}{Settings.CustomTag}");
                        MessageSent(message);

                        SetWaitingStatus(false);
                    }
                    catch (TimeoutException t)
                    {
                        ErrorMessage($"Send Message Timeout.");
                    }
                    catch (Exception e)
                    {
                        ErrorMessage($"Error sending message: {e.Message}");
                    }
                    finally
                    {
                        SetWaitingStatus(false);
                    }
                });
            }
            else
            {
                ErrorMessage($"SerialPort is not connected.");
            }
        }

        public string ReadRawText()
        {
            int nBytesToRead = SerialPort.BytesToRead;
            byte[] buffer = new byte[nBytesToRead];
            if (SerialPort.Read(buffer, 0, nBytesToRead) > nBytesToRead)
            {
                ErrorMessage("More data was read than what was supposed to be read.");
            }

            return SerialPort.Encoding.GetString(buffer);
        }

        public string ReadLine()
        {
            int nBytesToRead = SerialPort.BytesToRead;
            byte[] buffer = new byte[nBytesToRead];

            if (SerialPort.Read(buffer, 0, nBytesToRead) > nBytesToRead)
            {
                ErrorMessage("More data was read than what was supposed to be read.");
            }

            string receivedData = (SerialPort.Encoding).GetString(buffer);
            ReceivedDataBuffer += receivedData;

            if (ReceivedDataBuffer[ReceivedDataBuffer.Length - 1] == '\n')
            {
                string received = ReceivedDataBuffer;
                ReceivedDataBuffer = "";
                return received;
            }

            return "";
        }

        public string ReadWithCustomTag()
        {
            int nBytesToRead = SerialPort.BytesToRead;
            byte[] buffer = new byte[nBytesToRead];

            if (SerialPort.Read(buffer, 0, nBytesToRead) > nBytesToRead)
            {
                ErrorMessage("More data was read than what was supposed to be read.");
            }

            string receivedData = (SerialPort.Encoding).GetString(buffer);
            ReceivedDataBuffer += receivedData;

            if (ReceivedDataBuffer.EndsWith(Settings.CustomTag))
            {
                string received = ReceivedDataBuffer;
                ReceivedDataBuffer = "";
                return received;
            }

            return "";
        }

        #endregion

        #region Buffer/Message clearing

        public void ClearReceivedMessages()
        {
            ReceivedMessages.Clear();
        }

        public void ClearSentMessages()
        {
            SentMessages.Clear();
        }

        public void ClearBuffers()
        {
            if (SerialPort.IsOpen)
            {
              SerialPort.DiscardInBuffer();
              SerialPort.DiscardOutBuffer();
            }
            else
            {
                ErrorMessage("Conenct to port to clear buffers.");
            }
        }

        #endregion

        #region Updating field and serialport values
        //Thread safe
        public void SetWaitingStatus(bool waiting)
        {
            Application.Current.Dispatcher.Invoke(() => WaitingStatus = waiting);
        }
        public void UpdateSerialValues()
        {
            UpdateSendTimeout(int.Parse(Math.Round(SendTimeout, 0).ToString()));
            UpdateReceiveTimeout(int.Parse(Math.Round(ReceiveTimeout, 0).ToString()));
            UpdateBufferSize(int.Parse(Math.Round(BufferSize, 0).ToString()));
            UpdateCOMName(COMName);
            UpdateBaudRate(int.Parse(Math.Round(double.Parse(DataBits), 0).ToString()));
            UpdateDataBits(int.Parse(Math.Round(double.Parse(DataBits), 0).ToString()));
            UpdateStopBits(StopBits);
            UpdateParity(Parity);
            UpdateHandShake(HandShake);
            UpdateDTR(DataTerminalReady);

            //MessageBox.Show(
            //    $"SendTimeout:    {SendTimeout}\n" +
            //    $"ReceiveTimeout: {ReceiveTimeout}\n" +
            //    $"BufferSize:     {BufferSize}\n" +
            //    $"COMName:        {COMName}\n" +
            //    $"ActiveCOMName:  {ActiveCOMName}\n" +
            //    $"BaudRate:       {BaudRate}\n" +
            //    $"DataBits:       {DataBits}\n" +
            //    $"StopBits:       {StopBits}\n" +
            //    $"Parity:         {Parity}\n" +
            //    $"HandShake:      {HandShake}\n" +
            //    $"DTR:            {DataTerminalReady}\n");
        }
        private void UpdateSendTimeout(int newVal)
        {
            if (!SerialPort.IsOpen) SerialPort.WriteTimeout = newVal;
        }
        private void UpdateReceiveTimeout(int newVal)
        {
            if (!SerialPort.IsOpen) SerialPort.ReadTimeout = newVal;
        }
        private void UpdateBufferSize(int newVal)
        {
            if (!SerialPort.IsOpen) SerialPort.ReadBufferSize = newVal; SerialPort.WriteBufferSize = newVal;
        }
        private void UpdateMaxReceivableMessages(int newVal)
        {
            if (!SerialPort.IsOpen) SerialPort.ReadBufferSize = newVal; SerialPort.WriteBufferSize = newVal;
        }
        private void UpdateCOMName(string newVal)
        {
            if (!SerialPort.IsOpen)
            {
                if (!string.IsNullOrEmpty(newVal))
                {
                    SerialPort.PortName = newVal;
                    ActiveCOMName = newVal;
                }
            }
        }
        private void UpdateBaudRate(int newVal)
        {
            if (!SerialPort.IsOpen) SerialPort.BaudRate = newVal;
        }
        private void UpdateDataBits(int newVal)
        {
            if (!SerialPort.IsOpen) SerialPort.DataBits = newVal;
        }
        private void UpdateStopBits(string newVal)
        {
            if (!SerialPort.IsOpen) SerialPort.StopBits = (StopBits)Enum.Parse(typeof(StopBits), newVal);
        }
        private void UpdateParity(string newVal)
        {
            if (!SerialPort.IsOpen) SerialPort.Parity = (Parity)Enum.Parse(typeof(Parity), newVal);
        }
        private void UpdateHandShake(string newVal)
        {
            if (!SerialPort.IsOpen) SerialPort.Handshake = (Handshake)Enum.Parse(typeof(Handshake), newVal);
        }
        private void UpdateDTR(bool newVal)
        {
            if (!SerialPort.IsOpen) SerialPort.DtrEnable = newVal;
        }

        #endregion
    }
}
