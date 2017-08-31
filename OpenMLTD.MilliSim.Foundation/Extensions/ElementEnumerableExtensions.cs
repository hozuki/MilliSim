using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace OpenMLTD.MilliSim.Foundation.Extensions {
    public static class ElementEnumerableExtensions {

        [NotNull]
        public static T Find<T>([NotNull, ItemNotNull] this IEnumerable<IElement> elements) where T : IElement {
            return elements.OfType<T>().First();
        }

        [CanBeNull]
        public static T FindOrNull<T>([NotNull, ItemNotNull] this IEnumerable<IElement> elements) where T : class, IElement {
            return elements.OfType<T>().FirstOrDefault();
        }

        [NotNull]
        public static T Find<T>([NotNull, ItemNotNull] this IEnumerable<IElement> elements, [NotNull] string name, StringComparison comparison) where T : IElement {
            return elements.OfType<T>().First(el => string.Compare(name, el.Name, comparison) == 0);
        }

        [NotNull]
        public static T Find<T>([NotNull, ItemNotNull] this IEnumerable<IElement> elements, [NotNull] string name) where T : IElement {
            return elements.Find<T>(name, StringComparison.Ordinal);
        }

        [CanBeNull]
        public static T FindOrNull<T>([NotNull, ItemNotNull] this IEnumerable<IElement> elements, [NotNull] string name, StringComparison comparison) where T : class, IElement {
            return elements.OfType<T>().FirstOrDefault(el => string.Compare(name, el.Name, comparison) == 0);
        }

        [CanBeNull]
        public static T FindOrNull<T>([NotNull, ItemNotNull] this IEnumerable<IElement> elements, [NotNull] string name) where T : class, IElement {
            return elements.FindOrNull<T>(name, StringComparison.Ordinal);
        }

        [NotNull]
        public static IElement Find([NotNull, ItemNotNull] this IEnumerable<IElement> elements, [NotNull] string name, StringComparison comparison) {
            return elements.First(el => string.Compare(name, el.Name, comparison) == 0);
        }

        [NotNull]
        public static IElement Find([NotNull, ItemNotNull] this IEnumerable<IElement> elements, [NotNull] string name) {
            return elements.Find(name, StringComparison.Ordinal);
        }

        [CanBeNull]
        public static IElement FindOrNull([NotNull, ItemNotNull] this IEnumerable<IElement> elements, [NotNull] string name, StringComparison comparison) {
            return elements.FirstOrDefault(el => string.Compare(name, el.Name, comparison) == 0);
        }

        [CanBeNull]
        public static IElement FindOrNull([NotNull, ItemNotNull] this IEnumerable<IElement> elements, [NotNull] string name) {
            return elements.FindOrNull(name, StringComparison.Ordinal);
        }

    }
}
