using System.Collections.Generic;
using JetBrains.Annotations;

namespace OpenMLTD.MilliSim.Core.Entities.Runtime {
    public sealed class RuntimeScore {

        public RuntimeScore([NotNull, ItemNotNull] IReadOnlyList<RuntimeNote> notes) {
            Notes = notes;
        }

        [NotNull, ItemNotNull]
        public IReadOnlyList<RuntimeNote> Notes { get; }

        public float OffsetToMusic { get; set; }

        public Difficulty Difficulty { get; set; }

    }
}
