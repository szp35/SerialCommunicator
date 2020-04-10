using SerialCommunicator.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerialCommunicator.ViewModels
{
    public class SerialMessageModel
    {
        public string RXorTX { get; set; }
        public string Message { get; set; }
        public DateTime Time { get; set; }
    }
}
