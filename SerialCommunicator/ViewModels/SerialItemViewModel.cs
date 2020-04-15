using SerialCommunicator.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerialCommunicator.ViewModels
{
    public class SerialItemViewModel : BaseViewModel
    {
        private SerialViewModel _serialView = new SerialViewModel();
        public SerialViewModel SerialView
        {
            get => _serialView;
            set => RaisePropertyChanged(ref _serialView, value);
        }

        public SerialItemViewModel(SerialViewModel serialView)
        {
            SerialView = serialView;
        }
        public SerialItemViewModel()
        {
            SerialView = new SerialViewModel(); ;
        }
    }
}
