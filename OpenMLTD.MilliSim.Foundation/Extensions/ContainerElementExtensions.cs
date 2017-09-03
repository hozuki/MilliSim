using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace OpenMLTD.MilliSim.Foundation.Extensions {
    public static class ContainerElementExtensions {

        [CanBeNull]
        public static T FindOrNull<T>(this IContainerElement container) where T : class, IElement {
            var b = TryFind<T>(container, out var r);
            return b ? r : null;
        }

        [NotNull]
        public static T Find<T>(this IContainerElement container) where T : IElement {
            var b = TryFind<T>(container, out var r);
            if (!b) {
                // Similar to Enumerable.Single().
                throw new InvalidOperationException();
            }
            return r;
        }

        public static bool TryFind<T>(this IContainerElement container, [CanBeNull] out T result) where T : IElement {
            var q = new Queue<IContainerElement>();
            q.Enqueue(container);

            while (q.Count > 0) {
                var c = q.Dequeue();

                if (c.Elements.Count == 0) {
                    result = default;
                    return false;
                }

                foreach (var element in c.Elements) {
                    if (element is T t) {
                        result = t;
                        return true;
                    }
                    if (element is IContainerElement c2) {
                        q.Enqueue(c2);
                    }
                }
            }

            result = default;
            return false;
        }

    }
}
