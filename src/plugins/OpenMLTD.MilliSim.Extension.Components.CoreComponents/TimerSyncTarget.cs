using System.Diagnostics;

namespace OpenMLTD.MilliSim.Extension.Components.CoreComponents {
    public enum TimerSyncTarget {

        /// <summary>
        /// Choose automatically.
        /// </summary>
        Auto = 0,
        /// <summary>
        /// Force using <see cref="OpenMLTD.MilliSim.Audio.Sound"/> provided by <see cref="BackgroundMusic.Music"/>.
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
