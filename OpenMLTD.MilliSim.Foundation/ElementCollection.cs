using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace OpenMLTD.MilliSim.Foundation {
    public sealed class ElementCollection : IReadOnlyList<IElement> {

        public ElementCollection()
            : this(null) {
        }

        public ElementCollection([CanBeNull, ItemNotNull] IReadOnlyList<IElement> elements) {
            _elements = elements ?? new Element[0];
        }

        public IEnumerator<IElement> GetEnumerator() {
            return _elements.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public int Count => _elements.Count;

        public IElement this[int index] => _elements[index];

        private readonly IReadOnlyList<IElement> _elements;

    }
}
