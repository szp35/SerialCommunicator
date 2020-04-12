using SerialCommunicator.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerialCommunicator.SerialFileLogger
{
    public static class MessageWriter
    {
        public static void WriteToFile(string filePath, List<SerialMessageModel> messages)
        {
            List<string> formattedMessages = new List<string>();

            foreach (SerialMessageModel message in messages)
            {
                formattedMessages.Add($"{message.RXorTX} >> {message.Message} || {message.Time}");
            }

            if (File.Exists(filePath))
                File.WriteAllLines(filePath, formattedMessages.ToArray());
            else
                throw new FileNotFoundException("File does not exist, couldn't write messages to file", filePath);
        }
    }
}
