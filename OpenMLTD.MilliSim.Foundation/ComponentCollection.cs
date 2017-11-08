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

        internal ComponentCollection([NotNull] IComponentContainer container, [CanBeNull, ItemNotNull] IReadOnlyList<IComponent> components) {
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
                if (!_components.Contains(item)) {
                    _components.Add(item);
                }
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
                var canBeDeleted = ValidateComponentDeletion(item);
                if (canBeDeleted) {
                    var entry = new PendingEntry {
                        Operation = Operation.Remove,
                        Item = item
                    };
                    _pendingQueue.Enqueue(entry);
                }
                return canBeDeleted;
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
                var lastIndex = _components.IndexOf(item);
                if (lastIndex < 0) {
                    _components.Insert(index, item);
                } else {
                    if (lastIndex < index) {
                        --index;
                    }
                    _components.RemoveAt(lastIndex);
                    _components.Insert(index, item);
                }
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

        public override string ToString() {
            var baseString = base.ToString();

            var c1 = _components.Count;
            var c2 = _pendingQueue.Count;

            return $"{baseString} (Count = {c1}, Pending = {c2})";
        }

        internal bool HasPendingEntries => _pendingQueue.Count > 0;

        public void ExecutePendingQueue() {
            if (!HasPendingEntries) {
                return;
            }

            ExecutePendingQueue(_components, _pendingQueue);
        }

        private static IList<IComponent> ExecutePendingQueue(IList<IComponent> c, Queue<PendingEntry> q) {
            while (q.Count > 0) {
                var entry = q.Dequeue();
                switch (entry.Operation) {
                    case Operation.Add:
                        if (!c.Contains(entry.Item)) {
                            c.Add(entry.Item);
                        }
                        break;
                    case Operation.Insert: {
                            var index = entry.Index;
                            var item = entry.Item;
                            var lastIndex = c.IndexOf(item);
                            if (lastIndex < 0) {
                                c.Insert(index, item);
                            } else {
                                if (lastIndex < index) {
                                    --index;
                                }
                                c.RemoveAt(lastIndex);
                                c.Insert(index, item);
                            }
                            break;
                        }
                    case Operation.Remove:
                        c.Remove(entry.Item);
                        break;
                    case Operation.RemoveAt:
                        c.RemoveAt(entry.Index);
                        break;
                    case Operation.Clear:
                        c.Clear();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return c;
        }

        private bool ValidateComponentDeletion([CanBeNull] IComponent item) {
            if (!HasPendingEntries) {
                return _components.Contains(item);
            }

            IList<IComponent> c = new List<IComponent>(_components);
            var q = new Queue<PendingEntry>(_pendingQueue);
            c = ExecutePendingQueue(c, q);

            return c.Contains(item);
        }

        private bool HasActiveEnumerators => _activeEnumeratorCount > 0;

        private readonly IList<IComponent> _components;
        private int _activeEnumeratorCount;

        private readonly Queue<PendingEntry> _pendingQueue = new Queue<PendingEntry>();

        private readonly SimpleUsingLock _enumeratorLock = new SimpleUsingLock();

    }
}
