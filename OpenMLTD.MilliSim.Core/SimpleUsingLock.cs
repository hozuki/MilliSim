using System;
using System.Threading;

namespace OpenMLTD.MilliSim.Core {
    /// <inheritdoc cref="DisposableBase"/>
    /// <summary>
    /// A simple wrapper of <see cref="ReaderWriterLockSlim"/> taking advantage of the <see langword="using"/> syntax.
    /// </summary>
    /// <remarks>
    /// http://www.cnblogs.com/blqw/p/3475734.html
    /// </remarks>
    /// <example>
    /// using (var @lock = simpleUsingLock.NewReadLock()) {
    ///     // ...
    /// }
    /// </example>
    public sealed class SimpleUsingLock : DisposableBase {

        /// <summary>
        /// Creates a new read lock.
        /// </summary>
        /// <returns>An <see cref="IDisposable"/> which is the read lock.</returns>
        public IDisposable NewReadLock() {
            if (_lockSlim.IsReadLockHeld || _lockSlim.IsWriteLockHeld) {
                return Disposable.Empty;
            } else {
                _lockSlim.EnterReadLock();
                return new InternalLock(_lockSlim, false);
            }
        }

        /// <summary>
        /// Creates a new write lock.
        /// </summary>
        /// <returns>An <see cref="IDisposable"/> which is the write lock.</returns>
        public IDisposable NewWriteLock() {
            if (_lockSlim.IsWriteLockHeld) {
                return Disposable.Empty;
            }

            if (_lockSlim.IsReadLockHeld) {
                throw new InvalidOperationException("Cannot obtain a write lock when engaged in read mode.");
            }

            _lockSlim.EnterWriteLock();
            return new InternalLock(_lockSlim, true);
        }

        /// <inheritdoc cref="DisposableBase.Dispose(bool)"/>
        protected override void Dispose(bool disposing) {
            if (disposing) {
                _lockSlim.Dispose();
            }
        }

        private readonly ReaderWriterLockSlim _lockSlim = new ReaderWriterLockSlim();

        private struct InternalLock : IDisposable {

            internal InternalLock(ReaderWriterLockSlim @lock, bool isWrite) {
                _lock = @lock;
                _isWrite = isWrite;
            }

            void IDisposable.Dispose() {
                if (_isWrite) {
                    if (_lock.IsWriteLockHeld) {
                        _lock.ExitWriteLock();
                    }
                } else {
                    if (_lock.IsReadLockHeld) {
                        _lock.ExitReadLock();
                    }
                }
            }

            private readonly ReaderWriterLockSlim _lock;
            private readonly bool _isWrite;

        }

    }
}
