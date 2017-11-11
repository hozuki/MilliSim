using OpenTK;
using OpenTK.Audio.OpenAL;

namespace OpenMLTD.MilliSim.Audio {
    public sealed class AudioListener {

        internal AudioListener() {
        }

        public float Volume {
            get {
                AL.GetListener(ALListenerf.Gain, out var value);
                return value;
            }
            set => AL.Listener(ALListenerf.Gain, value);
        }

        public Vector3 Position {
            get {
                AL.GetListener(ALListener3f.Position, out var value);
                return value;
            }
            set => AL.Listener(ALListener3f.Position, value.X, value.Y, value.Z);
        }

        public Vector3 Velocity {
            get {
                AL.GetListener(ALListener3f.Velocity, out var value);
                return value;
            }
            set => AL.Listener(ALListener3f.Velocity, value.X, value.Y, value.Z);
        }

        public Vector3 OrientationAt {
            get {
                AL.GetListener(ALListenerfv.Orientation, out var value, out var _);
                _orientationAt = value;
                return value;
            }
            set {
                _orientationAt = value;
                SetOrientationValues();
            }
        }

        public Vector3 OrientationUp {
            get {
                AL.GetListener(ALListenerfv.Orientation, out var _, out var value);
                _orientationUp = value;
                return value;
            }
            set {
                _orientationUp = value;
                SetOrientationValues();
            }
        }

        private void SetOrientationValues() {
            var at = _orientationAt;
            var up = _orientationUp;
            AL.Listener(ALListenerfv.Orientation, ref at, ref up);
        }

        private Vector3 _orientationAt = Vector3.Zero;
        private Vector3 _orientationUp = Vector3.UnitZ;

    }
}
