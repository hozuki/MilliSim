using System;

namespace OpenMLTD.MilliSim.Core {
    public sealed class Disposable : IDisposable {

        private Disposable() {
        }

        public static readonly Disposable Empty = new Disposable();

        void IDisposable.Dispose() {
        }

    }
}
