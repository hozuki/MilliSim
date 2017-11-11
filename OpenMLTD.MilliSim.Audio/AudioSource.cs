using System;
using JetBrains.Annotations;
using OpenTK;
using OpenTK.Audio.OpenAL;

namespace OpenMLTD.MilliSim.Audio {
    public sealed class AudioSource : AudioObject {

        public AudioSource([NotNull] AudioContext context) {
            Context = context;
            Alc.MakeContextCurrent(context.NativeContext);
            AL.GenSource(out _source);
        }

        public AudioContext Context { get; }

        public bool IsLooped {
            get {
                AL.GetSource(NativeSource, ALSourceb.Looping, out var value);
                return value;
            }
            set => AL.Source(NativeSource, ALSourceb.Looping, value);
        }

        public TimeSpan CurrentTime {
            get {
                AL.GetSource(NativeSource, ALSourcef.SecOffset, out var value);
                return TimeSpan.FromSeconds(value);
            }
            set {
                if (value < TimeSpan.Zero) {
                    value = TimeSpan.Zero;
                }
                AL.Source(NativeSource, ALSourcef.SecOffset, (float)value.TotalSeconds);
            }
        }

        public float Volume {
            get {
                AL.GetSource(NativeSource, ALSourcef.Gain, out var value);
                return value;
            }
            set => AL.Source(NativeSource, ALSourcef.Gain, value);
        }

        public float Pitch {
            get {
                AL.GetSource(NativeSource, ALSourcef.Pitch, out var value);
                return value;
            }
            set => AL.Source(NativeSource, ALSourcef.Pitch, value);
        }

        public void Play() {
            IsLooped = false;
            PlayDirect();
        }

        public void PlayDirect() {
            AL.SourcePlay(NativeSource);
        }

        public void PlayLooped() {
            IsLooped = true;
            PlayDirect();
        }

        public void Pause() {
            AL.SourcePause(NativeSource);
        }

        public void Stop() {
            AL.SourceStop(NativeSource);
        }

        public void QueueBuffer([NotNull] AudioBuffer buffer) {
            var s = unchecked((int)NativeSource);
            var b = unchecked((int)buffer.NativeBuffer);
            AL.SourceQueueBuffer(s, b);
        }

        public void UnqueueBuffer() {
            var s = unchecked((int)NativeSource);
            AL.SourceUnqueueBuffer(s);
        }

        public AudioState State {
            get {
                var state = AlState;
                switch (state) {
                    case ALSourceState.Initial:
                        return AudioState.Loaded;
                    case ALSourceState.Playing:
                        return AudioState.Playing;
                    case ALSourceState.Paused:
                        return AudioState.Paused;
                    case ALSourceState.Stopped:
                        return AudioState.Stopped;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        internal uint NativeSource => _source;

        internal int ByteOffset {
            get {
                AL.GetSource(NativeSource, ALGetSourcei.ByteOffset, out var value);
                return value;
            }
            set => AL.Source(NativeSource, ALSourcei.ByteOffset, value);
        }

        internal int SampleOffset {
            get {
                AL.GetSource(NativeSource, ALGetSourcei.SampleOffset, out var value);
                return value;
            }
            set => AL.Source(NativeSource, ALSourcei.SampleOffset, value);
        }

        internal int BuffersProcessed {
            get {
                AL.GetSource(NativeSource, ALGetSourcei.BuffersProcessed, out var value);
                return value;
            }
        }

        internal int BuffersQueued {
            get {
                AL.GetSource(NativeSource, ALGetSourcei.BuffersQueued, out var value);
                return value;
            }
        }

        internal Vector3 Position {
            get {
                AL.GetSource(NativeSource, ALSource3f.Position, out var value);
                return value;
            }
            set => AL.Source(NativeSource, ALSource3f.Position, value.X, value.Y, value.Z);
        }

        internal Vector3 Velocity {
            get {
                AL.GetSource(NativeSource, ALSource3f.Velocity, out var value);
                return value;
            }
            set => AL.Source(NativeSource, ALSource3f.Velocity, value.X, value.Y, value.Z);
        }

        internal Vector3 Direction {
            get {
                AL.GetSource(NativeSource, ALSource3f.Direction, out var value);
                return value;
            }
            set => AL.Source(NativeSource, ALSource3f.Direction, value.X, value.Y, value.Z);
        }

        internal bool IsSourceRelative {
            get {
                AL.GetSource(NativeSource, ALSourceb.SourceRelative, out var value);
                return value;
            }
            set => AL.Source(NativeSource, ALSourceb.SourceRelative, value);
        }

        internal void Rewind() {
            AL.SourceRewind(NativeSource);
        }

        internal ALSourceState AlState {
            get {
                if (_source == 0) {
                    throw new InvalidOperationException();
                }

                var state = AL.GetSourceState(_source);
                return state;
            }
        }

        internal ALSourceType SourceType {
            get {
                if (_source == 0) {
                    throw new InvalidOperationException();
                }

                var sourceType = AL.GetSourceType(_source);
                return sourceType;
            }
        }

        protected override void Dispose(bool disposing) {
            AL.DeleteSource(ref _source);
        }

        private uint _source;

    }
}
