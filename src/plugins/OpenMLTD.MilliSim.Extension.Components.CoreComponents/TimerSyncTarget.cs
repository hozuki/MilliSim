using System.Diagnostics;
using OpenMLTD.MilliSim.Audio;

namespace OpenMLTD.MilliSim.Extension.Components.CoreComponents {
    /// <summary>
    /// The target that <see cref="SyncTimer"/> synchronize to.
    /// </summary>
    public enum TimerSyncTarget {

        /// <summary>
        /// Choose automatically.
        /// </summary>
        Auto = 0,
        /// <summary>
        /// Force using <see cref="Sound"/> provided by <see cref="BackgroundMusic.Music"/>.
        /// </summary>
        Audio = 1,
        /// <summary>
        /// Force using <see cref="BackgroundVideo.CurrentTime"/>.
        /// </summary>
        Video = 2,
        /// <summary>
        /// Force using <see cref="Microsoft.Xna.Framework.GameTime"/> provided by game loop.
        /// </summary>
        GameTime = 3,
        /// <summary>
        /// Use an internal <see cref="Stopwatch"/>.
        /// </summary>
        Stopwach = 4

    }
}
