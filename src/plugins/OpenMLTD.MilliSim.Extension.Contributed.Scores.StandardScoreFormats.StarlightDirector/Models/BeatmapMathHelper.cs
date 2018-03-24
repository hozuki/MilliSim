using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace OpenMLTD.MilliSim.Extension.Contributed.Scores.StandardScoreFormats.StarlightDirector.Models {
    public static class BeatmapMathHelper {

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double BpmToInterval(double bpm) {
            return 60 / bpm;
        }

    }
}
