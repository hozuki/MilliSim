using System.Collections.Generic;
using JetBrains.Annotations;

namespace OpenMLTD.MilliSim.Core.Entities.Runtime {
    public sealed class RuntimeScore {

        internal RuntimeScore([NotNull, ItemNotNull] IReadOnlyList<RuntimeNote> notes) {
            Notes = notes;
        }

        [NotNull, ItemNotNull]
        public IReadOnlyList<RuntimeNote> Notes { get; }

        public float OffsetToMusic { get; internal set; }

        public Difficulty Difficulty { get; internal set; }

    }
}
