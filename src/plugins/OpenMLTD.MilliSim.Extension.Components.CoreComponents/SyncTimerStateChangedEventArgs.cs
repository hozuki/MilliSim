using System;
using Microsoft.Xna.Framework.Media;

namespace OpenMLTD.MilliSim.Extension.Components.CoreComponents {
    /// <inheritdoc />
    /// <summary>
    /// Event arguments for <see cref="SyncTimer.StateChanged"/>. This class cannot be inherited.
    /// </summary>
    public sealed class SyncTimerStateChangedEventArgs : EventArgs {

        /// <inheritdoc />
        /// <summary>
        /// Creates a new <see cref="SyncTimerStateChangedEventArgs"/> instance.
        /// </summary>
        /// <param name="newState">The new state.</param>
        internal SyncTimerStateChangedEventArgs(MediaState newState) {
            NewState = newState;
        }

        /// <summary>
        /// Gets the new state.
        /// </summary>
        public MediaState NewState { get; }

    }
}
