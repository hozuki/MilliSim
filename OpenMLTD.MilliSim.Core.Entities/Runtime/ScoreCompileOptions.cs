namespace OpenMLTD.MilliSim.Core.Entities.Runtime {
    public sealed class ScoreCompileOptions : FlexibleOptions {

        public ScoreCompileOptions() {
            Difficulty = Difficulty.D2Mix;
            GlobalSpeed = 1;
        }

        /// <summary>
        /// The selected <see cref="Difficulty"/> to use. A single MLTD score contains multiple difficulties, but the player plays only one of them.
        /// </summary>
        public Difficulty Difficulty {
            get => GetValue<Difficulty>(DifficultyKey);
            set => SetValue(DifficultyKey, value);
        }

        public static string DifficultyKey => nameof(Difficulty);

        /// <summary>
        /// The global speed multiplier.
        /// </summary>
        public float GlobalSpeed {
            get => GetValue<float>(GlobalSpeedKey);
            set => SetValue(GlobalSpeedKey, value);
        }

        public static string GlobalSpeedKey => nameof(GlobalSpeed);

        public static readonly ScoreCompileOptions Default = new ScoreCompileOptions();

    }
}
