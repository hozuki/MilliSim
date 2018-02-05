using System.Collections.Generic;
using JetBrains.Annotations;

namespace OpenMLTD.MilliSim.Contributed.Scores.Runtime {
    public class RuntimeScore {

        public RuntimeScore([NotNull, ItemNotNull] IReadOnlyList<RuntimeNote> notes) {
            Notes = notes;
        }

        [NotNull, ItemNotNull]
        public IReadOnlyList<RuntimeNote> Notes { get; }

        public int TrackCount { get; set; }

    }
}
