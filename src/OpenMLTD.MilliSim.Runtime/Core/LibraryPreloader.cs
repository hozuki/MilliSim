using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using OpenMLTD.MilliSim.Core.Extensions;
using OpenMLTD.MilliSim.Core.Interop;

namespace OpenMLTD.MilliSim.Core {
    public static class LibraryPreloader {

        /// <summary>
        /// Preloads a dynamic library file (.dll) into the memory space of current process.
        /// According to the file target architecture, it should be placed at lib/x86 or lib/x64 under current working directory.
        /// </summary>
        /// <param name="libraryFileName">The file name of this library, including file extension.</param>
        /// <returns><see langword="true"/> if it is successfully preloaded; otherwise, <see langword="false"/>.</returns>
        public static bool PreloadLibrary(string libraryFileName) {
            var path = Environment.CurrentDirectory;
            var ptrSize = Marshal.SizeOf(typeof(IntPtr));
            string childDirName;
            switch (ptrSize) {
                case 4:
                    childDirName = ChildDirectory32;
                    break;
                case 8:
                    childDirName = ChildDirectory64;
                    break;
                default:
                    throw new PlatformNotSupportedException($"An OS whose pointer size is {ptrSize} is not supported right now.");
            }
            path = Path.Combine(path, BaseDirectory, childDirName, libraryFileName);

            string key;
            if (Environment.OSVersion.Platform == PlatformID.Win32NT) {
                key = path.ToLowerInvariant();
            } else {
                key = path;
            }

            if (PreloadedLibraryHandles.ContainsKey(key)) {
                return true;
            }

            var handle = NativeMethods.LoadLibrary(path);
            var successful = handle != IntPtr.Zero;

            if (successful) {
                PreloadedLibraryHandles.Add(key, handle);
            }

            return successful;
        }

        public static void UnloadAllPreloadedLibraries() {
            foreach (var (_, handle) in PreloadedLibraryHandles) {
                NativeMethods.FreeLibrary(handle);
            }
            PreloadedLibraryHandles.Clear();
        }

        private static readonly string BaseDirectory = "lib";
        private static readonly string ChildDirectory32 = "x86";
        private static readonly string ChildDirectory64 = "x64";

        private static readonly Dictionary<string, IntPtr> PreloadedLibraryHandles = new Dictionary<string, IntPtr>();

    }
}
