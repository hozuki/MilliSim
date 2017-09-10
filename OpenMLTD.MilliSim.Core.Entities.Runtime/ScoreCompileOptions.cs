namespace OpenMLTD.MilliSim.Core.Entities.Runtime {
    public sealed class ScoreCompileOptions {

        /// <summary>
        /// The selected <see cref="Difficulty"/> to use. A single MLTD score contains multiple difficulties, but the player plays only one of them.
        /// </summary>
        public Difficulty Difficulty { get; set; } = Difficulty.D2Mix;

        /// <summary>
        /// The global speed multiplier.
        /// </summary>
        public float GlobalSpeed { get; set; } = 1f;

        internal static readonly ScoreCompileOptions Default = new ScoreCompileOptions();

    }
}
