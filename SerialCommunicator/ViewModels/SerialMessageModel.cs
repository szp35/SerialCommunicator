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
        /// <summary>
        /// A number that increases as item count increases. bind to listbox.count
        /// </summary>
        public string CountIndex { get; set; }
        public string RXorTX { get; set; }
        public string Message { get; set; }
        public DateTime Time { get; set; }
    }
}
