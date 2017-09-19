using System.Collections.Generic;
using JetBrains.Annotations;

namespace OpenMLTD.MilliSim.Core.Entities.Runtime {
    public class RuntimeScore {

        public RuntimeScore([NotNull, ItemNotNull] IReadOnlyList<RuntimeNote> notes) {
            Notes = notes;
        }

        [NotNull, ItemNotNull]
        public IReadOnlyList<RuntimeNote> Notes { get; }

        public double OffsetToMusic { get; set; }

        public int TrackCount { get; set; }

    }
}
