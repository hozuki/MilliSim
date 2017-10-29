using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Core;

namespace OpenMLTD.MilliSim.Foundation {
    public sealed partial class ComponentCollection : IList<IComponent> {

        internal ComponentCollection([NotNull] IComponentContainer container)
            : this(container, null) {
        }

        public ComponentCollection([NotNull] IComponentContainer container, [CanBeNull, ItemNotNull] IReadOnlyList<IComponent> components) {
            Container = container ?? throw new ArgumentNullException(nameof(container));
            _components = components == null ? new List<IComponent>() : new List<IComponent>(components);
        }

        [NotNull]
        public IComponentContainer Container { get; }

        public IEnumerator<IComponent> GetEnumerator() {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public void Add(IComponent item) {
            if (HasActiveEnumerators) {
                var entry = new PendingEntry {
                    Operation = Operation.Add,
                    Item = item
                };
                _pendingQueue.Enqueue(entry);
            } else {
                _components.Add(item);
            }
        }

        public void Clear() {
            if (HasActiveEnumerators) {
                var entry = new PendingEntry {
                    Operation = Operation.Clear
                };
                _pendingQueue.Enqueue(entry);
            } else {
                _components.Clear();
            }
        }

        public bool Contains(IComponent item) {
            return _components.Contains(item);
        }

        public void CopyTo(IComponent[] array, int arrayIndex) {
            _components.CopyTo(array, arrayIndex);
        }

        public bool Remove(IComponent item) {
            if (HasActiveEnumerators) {
                var entry = new PendingEntry {
                    Operation = Operation.Remove,
                    Item = item
                };
                _pendingQueue.Enqueue(entry);
                return ValidateComponentDeletion(item);
            } else {
                return _components.Remove(item);
            }
        }

        public int Count => _components.Count;

        public bool IsReadOnly => _components.IsReadOnly;

        public int IndexOf(IComponent item) {
            return _components.IndexOf(item);
        }

        public void Insert(int index, IComponent item) {
            if (HasActiveEnumerators) {
                var entry = new PendingEntry {
                    Operation = Operation.Insert,
                    Index = index,
                    Item = item
                };
                _pendingQueue.Enqueue(entry);
            } else {
                _components.Insert(index, item);
            }
        }

        public void RemoveAt(int index) {
            if (HasActiveEnumerators) {
                var entry = new PendingEntry {
                    Operation = Operation.RemoveAt,
                    Index = index
                };
                _pendingQueue.Enqueue(entry);
            } else {
                _components.RemoveAt(index);
            }
        }

        public IComponent this[int index] {
            get => _components[index];
            set => _components[index] = value;
        }

        internal bool HasPendingEntries => _pendingQueue.Count > 0;

        internal void ExecutePendingQueue() {
            if (!HasPendingEntries) {
                return;
            }

            while (HasPendingEntries) {
                var item = _pendingQueue.Dequeue();
                throw new NotImplementedException();
            }
        }

        private bool ValidateComponentDeletion([CanBeNull] IComponent item) {
            if (!HasPendingEntries) {
                return false;
            }

            foreach (var entry in _pendingQueue) {
                throw new NotImplementedException();
            }

            throw new NotImplementedException();
        }

        private bool HasActiveEnumerators => _activeEnumeratorCount > 0;

        private readonly IList<IComponent> _components;
        private int _activeEnumeratorCount;

        private readonly Queue<PendingEntry> _pendingQueue = new Queue<PendingEntry>();

        private readonly SimpleUsingLock _enumeratorLock = new SimpleUsingLock();

    }
}
