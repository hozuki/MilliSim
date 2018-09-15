namespace OpenMLTD.MilliSim.Foundation {
    partial class BaseGameComponentCollection {

        /// <summary>
        /// Operations for entries in the update queue.
        /// </summary>
        private enum Operation {

            /// <summary>
            /// Invalid operation.
            /// </summary>
            Invalid,
            /// <summary>
            /// Adds an item to the end of the list.
            /// </summary>
            Add,
            /// <summary>
            /// Inserts an item to a specified position.
            /// </summary>
            Insert,
            /// <summary>
            /// Removes an item by reference.
            /// </summary>
            Remove,
            /// <summary>
            /// Removes an item by index.
            /// </summary>
            RemoveAt,
            /// <summary>
            /// Clears the list.
            /// </summary>
            Clear

        }

    }
}
