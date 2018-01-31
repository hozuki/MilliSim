using System;
using System.Collections;
using System.Collections.Generic;

namespace OpenMLTD.MilliSim.Foundation {
    partial class BaseGameComponentCollection {

        public sealed class Enumerator : IEnumerator<IBaseGameComponent> {

            internal Enumerator(BaseGameComponentCollection collection) {
                _collection = collection;
                using (_collection._enumeratorLock.NewWriteLock()) {
                    ++_collection._activeEnumeratorCount;
                }
            }

            public void Dispose() {
                using (_collection._enumeratorLock.NewWriteLock()) {
                    --_collection._activeEnumeratorCount;
                }
            }

            public bool MoveNext() {
                var canMove = _index < _collection._components.Count - 1;
                if (canMove) {
                    ++_index;
                }
                return canMove;
            }

            public void Reset() {
                _index = -1;
            }

            public IBaseGameComponent Current {
                get {
                    var elements = _collection._components;
                    if (_index < 0 || elements.Count <= _index) {
                        throw new ArgumentOutOfRangeException();
                    }
                    return elements[_index];
                }
            }

            object IEnumerator.Current => Current;

            private int _index = -1;
            private readonly BaseGameComponentCollection _collection;

        }

    }
}
