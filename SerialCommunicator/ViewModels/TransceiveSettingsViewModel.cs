using SerialCommunicator.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerialCommunicator.ViewModels
{
    public class TransceiveSettingsViewModel : BaseViewModel
    {
        private bool _sendWithNewLine;
        private bool _receiveWithNewLine;

        private string _customTag;
        private bool _sendWithCustomTag;
        private bool _receiveWithCustomTag;

        private bool _sendWithNothingElse;
        private bool _receiveWithNothingElse;

        public bool SendWithNewLine
        {
            get => _sendWithNewLine;
            set => RaisePropertyChanged(ref _sendWithNewLine, value);
        }
        public bool ReceiveWithNewLine
        {
            get => _receiveWithNewLine;
            set => RaisePropertyChanged(ref _receiveWithNewLine, value);
        }

        public string CustomTag
        {
            get => _customTag;
            set => RaisePropertyChanged(ref _customTag, value);
        }
        public bool SendWithCustomTag
        {
            get => _sendWithCustomTag;
            set => RaisePropertyChanged(ref _sendWithCustomTag, value);
        }
        public bool ReceiveWithCustomTag
        {
            get => _receiveWithCustomTag;
            set => RaisePropertyChanged(ref _receiveWithCustomTag, value);
        }

        public bool SendWithNothingElse
        {
            get => _sendWithNothingElse;
            set => RaisePropertyChanged(ref _sendWithNothingElse, value);
        }
        public bool ReceiveWithNothingElse
        {
            get => _receiveWithNothingElse;
            set => RaisePropertyChanged(ref _receiveWithNothingElse, value);
        }

        public TransceiveSettingsViewModel()
        {
            SendWithNewLine = true;
            ReceiveWithNewLine = true;
            CustomTag = @"\n";
        }
    }
}
