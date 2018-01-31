namespace OpenMLTD.MilliSim.Extension.Components.CoreComponents {
    public enum TimerSyncTarget {

        /// <summary>
        /// Choose automatically.
        /// </summary>
        Auto,
        /// <summary>
        /// Force using <see cref="OpenMLTD.MilliSim.Audio.Sound"/> provided by <see cref="AudioController.Music"/>.
        /// </summary>
        Audio,
        /// <summary>
        /// Force using <see cref="BackgroundVideo.CurrentTime"/>.
        /// </summary>
        Video,
        /// <summary>
        /// Force using <see cref="Microsoft.Xna.Framework.GameTime"/> provided by game loop.
        /// </summary>
        GameTime

    }
}
