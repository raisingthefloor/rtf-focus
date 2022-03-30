using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Morphic.Focus
{
    internal static class UnsafeNative
    {
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [StructLayout(LayoutKind.Sequential)]
        private struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public int cbData;

            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpData;
        }

        private const int WM_COPYDATA = 0x004A;

        public static string GetMessage(int message, IntPtr lParam)
        {
            if (message == UnsafeNative.WM_COPYDATA)
            {
                try
                {
                    var data = Marshal.PtrToStructure<UnsafeNative.COPYDATASTRUCT>(lParam);
                    var result = string.Copy(data.lpData);
                    return result;
                }
                catch
                {
                    return null;
                }
            }

            return null;
        }

        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, ref COPYDATASTRUCT lParam);

        public static void SendMessage(IntPtr hwnd, string message)
        {
            var messageBytes = Encoding.Unicode.GetBytes(message);
            var data = new UnsafeNative.COPYDATASTRUCT
            {
                dwData = IntPtr.Zero,
                lpData = message,
                cbData = messageBytes.Length + 1 /* +1 because of \0 string termination */
            };

            if (UnsafeNative.SendMessage(hwnd, WM_COPYDATA, IntPtr.Zero, ref data) != 0)
                throw new Win32Exception(Marshal.GetLastWin32Error());
        }


        #region ntdll.dll (reverse-engineered)

        [StructLayout(LayoutKind.Sequential)]
        internal struct WNF_STATE_NAME
        {
            public uint data0;
            public uint data1;
        }

        //[StructLayout(LayoutKind.Sequential)]
        //internal struct WNF_TYPE_ID
        //{
        //    public Guid TypeId;
        //}

        // see third-party Rust declaration for function at: https://docs.rs/ntapi/0.3.7/ntapi/ntzwapi/fn.ZwUpdateWnfStateData.html
        // NOTE: we use the Nw variant instead of the Zw variant; Zw variants are _usually_ reserved for kernel mode only
        [DllImport("ntdll.dll")]
        //internal static extern int NtUpdateWnfStateData(ref WNF_STATE_NAME StateName, IntPtr Buffer, uint Length, ref WNF_TYPE_ID TypeId, IntPtr ExplicitScope, uint MatchingChangeStamp, uint CheckStamp);
        internal static extern int NtUpdateWnfStateData(ref WNF_STATE_NAME StateName, IntPtr Buffer, uint Length, IntPtr TypeId, IntPtr ExplicitScope, uint MatchingChangeStamp, uint CheckStamp);

        // see third-party declaration at: https://chromium.googlesource.com/external/github.com/DynamoRIO/drmemory/+/refs/heads/master/wininc/ntexapi.h
        // see third-party notes at: https://habr.com/ru/post/459626/
        // NOTE: we use the Nw variant instead of the Zw variant; Zw variants are _usually_ reserved for kernel mode only
        [DllImport("ntdll.dll")]
        internal static extern int NtQueryWnfStateData([In] ref WNF_STATE_NAME StateName, IntPtr TypeId, IntPtr ExplicitScope, out uint ChangeStamp, [Out] IntPtr Buffer, ref uint BufferSize);

        #endregion ntdll.dll (reverse-engineered)
    }
}
