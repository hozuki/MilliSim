using System;
using System.Runtime.InteropServices;

namespace OpenMLTD.MilliSim.Core.Interop {
    internal static class NativeMethods {

        [DllImport("kernel32", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

        [DllImport("kernel32", CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool QueryPerformanceFrequency(out long lpFrequency);

        [DllImport("kernel32", SetLastError = false, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        internal static extern IntPtr LoadLibrary(string lpLibFileName);

        [DllImport("kernel32", SetLastError = false, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool FreeLibrary(IntPtr hModule);

    }
}
