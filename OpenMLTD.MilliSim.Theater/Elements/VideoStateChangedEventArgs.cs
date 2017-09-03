using System;
using SharpDX.MediaFoundation;

namespace OpenMLTD.MilliSim.Theater.Elements {
    public sealed class VideoStateChangedEventArgs : EventArgs {

        internal VideoStateChangedEventArgs(MediaEngineEvent @event) {
            Event = @event;
        }

        public MediaEngineEvent Event { get; }

    }
}
