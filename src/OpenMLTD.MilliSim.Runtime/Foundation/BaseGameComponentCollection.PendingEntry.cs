using JetBrains.Annotations;

namespace OpenMLTD.MilliSim.Foundation {
    partial class BaseGameComponentCollection {

        /// <summary>
        /// An entry in the pending update list.
        /// </summary>
        private struct PendingEntry {

            /// <summary>
            /// The operation to apply.
            /// </summary>
            internal Operation Operation { get; set; }

            /// <summary>
            /// The item in the entry.
            /// This property can be used in <see cref="BaseGameComponentCollection.Operation.Add"/>,
            /// <see cref="BaseGameComponentCollection.Operation.Insert"/>, and <see cref="BaseGameComponentCollection.Operation.Remove"/>.
            /// </summary>
            [CanBeNull]
            internal IBaseGameComponent Item { get; set; }

            /// <summary>
            /// The index in the entry.
            /// This property can be used in <see cref="BaseGameComponentCollection.Operation.Insert"/> and
            /// <see cref="BaseGameComponentCollection.Operation.RemoveAt"/>.
            /// </summary>
            internal int Index { get; set; }

        }

    }
}
