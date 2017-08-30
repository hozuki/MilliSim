using System.ComponentModel;
using OpenMLTD.MilliSim.Core.Interop;

namespace OpenMLTD.MilliSim.Core {
    public static class PerformanceCounter {

        public static long GetCurrent() {
            var r = NativeMethods.QueryPerformanceCounter(out var result);
            if (!r) {
                throw new Win32Exception("QueryPerformanceCounter is not supported.");
            }
            return result;
        }

        // Result is in milliseconds.
        public static double GetDuration(long start, long stop) {
            if (_frequency == 0) {
                var r = NativeMethods.QueryPerformanceFrequency(out _frequency);
                if (!r) {
                    throw new Win32Exception("QueryPerformanceFrequency is not supported.");
                }
            }
            var duration = stop - start;
            var result = duration * Multiplier / _frequency;
            return result;
        }

        private static long _frequency;
        private const double Multiplier = 1e3;

    }
}
