namespace OpenMLTD.MilliSim.Foundation {
    partial class ComponentCollection {

        private struct PendingEntry {

            internal Operation Operation { get; set; }

            internal IComponent Item { get; set; }

            internal int Index { get; set; }

        }

    }
}
