using OpenMLTD.MilliSim.Theater.Elements.Visual.Background;

namespace OpenMLTD.MilliSim.Theater.Elements.Logical {
    public enum TimerSyncTarget {

        /// <summary>
        /// Choose automatically.
        /// </summary>
        Auto,
        /// <summary>
        /// Force using <see cref="OpenMLTD.MilliSim.Audio.Music.CurrentTime"/> provided by <see cref="AudioController.Music"/>.
        /// </summary>
        Audio,
        /// <summary>
        /// Force using <see cref="BackgroundVideo.CurrentTime"/>.
        /// </summary>
        Video,
        /// <summary>
        /// Force using <see cref="OpenMLTD.MilliSim.Core.GameTime"/> provided by game loop.
        /// </summary>
        GameTime

    }
}
