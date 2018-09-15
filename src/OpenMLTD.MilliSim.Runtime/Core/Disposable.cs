using System;

namespace OpenMLTD.MilliSim.Core {
    /// <summary>
    /// The most simple implementation of <see cref="IDisposable"/>.
    /// </summary>
    internal sealed class Disposable : IDisposable {

        private Disposable() {
        }

        /// <summary>
        /// The global <see cref="Disposable"/> instance.
        /// </summary>
        public static readonly Disposable Empty = new Disposable();

        void IDisposable.Dispose() {
        }

    }
}
