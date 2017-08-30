using System.Runtime.InteropServices;

namespace OpenMLTD.MilliSim.Core.Interop {
    internal static class NativeMethods {

        [DllImport("kernel32")]
        public static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

        [DllImport("kernel32")]
        public static extern bool QueryPerformanceFrequency(out long lpFrequency);

    }
}
