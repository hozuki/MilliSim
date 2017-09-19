namespace OpenMLTD.MilliSim.Core.Entities.Runtime {
    public class ScoreCompileOptions : Dynamic {

        public ScoreCompileOptions() {
            GlobalSpeed = 1;
        }

        /// <summary>
        /// The global speed multiplier.
        /// </summary>
        public float GlobalSpeed {
            get => GetValue<float>(GlobalSpeedKey);
            set => SetValue(GlobalSpeedKey, value);
        }

        public static string GlobalSpeedKey => nameof(GlobalSpeed);

    }
}
