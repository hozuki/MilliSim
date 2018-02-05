using System;
using System.Runtime.InteropServices;

namespace OpenMLTD.MilliSim.Core {
    public static class InteropHelper {

        public static byte[] StructureToBytes<T>(T obj) where T : struct {
            var len = Marshal.SizeOf(obj);
            var buf = new byte[len];
            var ptr = Marshal.AllocHGlobal(len);
            Marshal.StructureToPtr(obj, ptr, true);
            Marshal.Copy(ptr, buf, 0, len);
            Marshal.FreeHGlobal(ptr);
            return buf;
        }

        public static T BytesToStructure<T>(byte[] data) where T : struct {
            if (data == null || data.Length == 0) {
                throw new ArgumentException("Byte array is not valid.");
            }

            var t = typeof(T);
            var size = Marshal.SizeOf(t);
            if (size < data.Length) {
                throw new ArgumentException($"Byte array is not long enough to marshal \"{t}\".");
            }

            var ptr = Marshal.AllocHGlobal(size);
            Marshal.Copy(data, 0, ptr, size);
            var result = (T)Marshal.PtrToStructure(ptr, t);
            Marshal.FreeHGlobal(ptr);

            return result;
        }

    }
}
