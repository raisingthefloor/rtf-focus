using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Morphic.Focus
{
    static class KeyboardSend
    {
        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        private const int KEYEVENTF_EXTENDEDKEY = 1;
        private const int KEYEVENTF_KEYUP = 2;

        public static void KeyDown(Key vKey)
        {
            keybd_event((byte)vKey, 0, KEYEVENTF_EXTENDEDKEY, 0);
        }

        public static void KeyUp(Key vKey)
        {
            keybd_event((byte)vKey, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
        }

        public static void OpenPowerBar()
        {
            keybd_event((byte)91, 0, KEYEVENTF_EXTENDEDKEY, 0); //Key Down - Win Key
            keybd_event((byte)88, 0, KEYEVENTF_EXTENDEDKEY, 0); //Key Down - X Key
            keybd_event((byte)91, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0); //Key Up - Win Key
            keybd_event((byte)88, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0); //Key Up - X Key
        }
    }
}
