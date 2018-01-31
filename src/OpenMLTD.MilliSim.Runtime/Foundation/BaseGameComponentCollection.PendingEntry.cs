namespace OpenMLTD.MilliSim.Foundation {
    partial class BaseGameComponentCollection {

        private struct PendingEntry {

            internal Operation Operation { get; set; }

            internal IBaseGameComponent Item { get; set; }

            internal int Index { get; set; }

        }

    }
}
