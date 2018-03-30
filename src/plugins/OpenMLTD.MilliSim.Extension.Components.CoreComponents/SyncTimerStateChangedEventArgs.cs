using System;
using Microsoft.Xna.Framework.Media;

namespace OpenMLTD.MilliSim.Extension.Components.CoreComponents {
    public sealed class SyncTimerStateChangedEventArgs : EventArgs {

        internal SyncTimerStateChangedEventArgs(MediaState newState) {
            NewState = newState;
        }

        public MediaState NewState { get; }

    }
}
