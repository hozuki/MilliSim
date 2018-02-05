using System;

namespace OpenMLTD.MilliSim.Contributed.Scores.Runtime {
    public static class RuntimeNoteComparisons {

        public static readonly Comparison<RuntimeNote> ByTime = (n1, n2) => n1.HitTime.CompareTo(n2.HitTime);

        public static readonly Comparison<RuntimeNote> ByTimeThenX = (n1, n2) => {
            var c1 = n1.HitTime.CompareTo(n2.HitTime);
            return c1 != 0 ? c1 : n1.EndX.CompareTo(n2.EndX);
        };

    }
}
