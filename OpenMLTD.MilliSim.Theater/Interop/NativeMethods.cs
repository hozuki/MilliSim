using System;
using System.Runtime.InteropServices;

namespace OpenMLTD.MilliSim.Theater.Interop {
    internal static class NativeMethods {

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        internal static extern int SendMessage(IntPtr hWnd, int uMsg, IntPtr wParam, ref NativeStructures.LVGROUP lParam);

    }
}
