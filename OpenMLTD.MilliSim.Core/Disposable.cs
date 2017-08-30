using System;

namespace OpenMLTD.MilliSim.Core {
    public sealed class Disposable : IDisposable {

        private Disposable() {
        }

        /// <summary> 空的可释放对象
        /// </summary>
        public static readonly Disposable Empty = new Disposable();

        /// <summary> 空的释放方法
        /// </summary>
        public void Dispose() {
        }

    }
}
