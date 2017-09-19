using System;
using SharpDX.MediaFoundation;

namespace OpenMLTD.MilliSim.Theater.Elements.Visual.Background {
    public sealed class VideoStateChangedEventArgs : EventArgs {

        internal VideoStateChangedEventArgs(MediaEngineEvent oldValidState, MediaEngineEvent newState) {
            OldValidState = oldValidState;
            NewState = newState;
        }

        /// <summary>
        /// The old valid state, a <see cref="MediaEngineEvent"/> with <see cref="MediaEngineEvent.TimeUpdate"/> filtered out.
        /// </summary>
        public MediaEngineEvent OldValidState { get; }

        public MediaEngineEvent NewState { get; }

    }
}
