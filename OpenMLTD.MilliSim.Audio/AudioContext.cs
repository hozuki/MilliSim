using JetBrains.Annotations;
using OpenTK;
using OpenTK.Audio.OpenAL;

namespace OpenMLTD.MilliSim.Audio {
    public sealed class AudioContext : AudioObject {

        public AudioContext([NotNull] AudioDevice device, [CanBeNull] int[] attribList) {
            _context = Alc.CreateContext(device.NativeDevice, attribList);
            Device = device;
        }

        public AudioContext([NotNull] AudioDevice device)
            : this(device, null) {
        }

        public AudioDevice Device { get; }

        public bool SetAsCurrent() {
            return Alc.MakeContextCurrent(NativeContext);
        }

        public static bool Reset() {
            return Alc.MakeContextCurrent(ContextHandle.Zero);
        }

        internal ContextHandle NativeContext => _context;

        protected override void Dispose(bool disposing) {
            if (_context != ContextHandle.Zero) {
                var currentContext = Alc.GetCurrentContext();
                if (currentContext == _context) {
                    Alc.MakeContextCurrent(ContextHandle.Zero);
                }

                Alc.DestroyContext(_context);
            }
            _context = ContextHandle.Zero;
        }

        private ContextHandle _context;

    }
}
