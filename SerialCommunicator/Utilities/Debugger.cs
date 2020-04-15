using SerialCommunicator.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerialCommunicator.Utilities
{
    public static class Debugger
    {
        private static DebugWindow DebugWindow = new DebugWindow();

        public static void Show()
        {
            DebugWindow.Show();
        }

        public static void Hide()
        {
            DebugWindow.Close();
        }

        public static void WriteSlot(int slot, string name, string value)
        {
            if (DebugWindow != null)
                DebugWindow.WriteSlot(slot, name, value);
        }
    }
}
