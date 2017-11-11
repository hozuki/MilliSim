using System;
using System.Linq;
using JetBrains.Annotations;
using OpenTK.Audio.OpenAL;

namespace OpenMLTD.MilliSim.Audio {
    public sealed class AudioDevice : AudioObject {

        public AudioDevice([CanBeNull] string deviceName) {
            DeviceName = deviceName;
            _device = Alc.OpenDevice(deviceName);
        }

        public AudioDevice()
            : this(DetermineDriver()) {
        }

        internal IntPtr NativeDevice => _device;

        internal string DeviceName { get; }

        protected override void Dispose(bool disposing) {
            if (_device != IntPtr.Zero) {
                Alc.CloseDevice(_device);
            }
            _device = IntPtr.Zero;
        }

        private static string DetermineDriver() {
            var platform = Environment.OSVersion.Platform;
            var availableDrivers = Drivers.Where(d => d.Platform == platform).Select(d => d.Driver).ToArray();

            if (availableDrivers.Length == 0) {
                return null;
            }

            foreach (var driver in availableDrivers) {
                var device = Alc.OpenDevice(driver);
                if (device != IntPtr.Zero) {
                    Alc.CloseDevice(device);
                    return driver;
                }
            }

            return null;
        }

        // https://www.openal.org/platforms/
        private static readonly (PlatformID Platform, string Driver)[] Drivers = {
            (PlatformID.Unix, "native"),
            (PlatformID.Unix, "OSS"),
            (PlatformID.Unix, "ALSA"),
            (PlatformID.MacOSX, "Core Audio"),
            (PlatformID.Win32NT, "DirectSound3D"), // actually WAS API?
            (PlatformID.Win32NT, "DirectSound"),
            (PlatformID.Win32NT, "MMSYSTEM")
        };

        private IntPtr _device;

    }
}
