using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace OpenMLTD.MilliSim.Foundation.Extensions {
    public static class ComponentContainerExtensions {

        [CanBeNull]
        public static T FindSingleOrNull<T>(this IBaseGameComponentContainer container) where T : class, IBaseGameComponent {
            var b = TryFind<T>(container, out var r);
            return b ? r : null;
        }

        public static T FindSingle<T>(this IBaseGameComponentContainer container) where T : IBaseGameComponent {
            var b = TryFind<T>(container, out var r);
            if (!b) {
                // Similar to Enumerable.Single().
                throw new InvalidOperationException();
            }
            return r;
        }

        public static bool TryFind<T>(this IBaseGameComponentContainer container, [CanBeNull] out T result) where T : IBaseGameComponent {
            var q = new Queue<IBaseGameComponentContainer>();
            q.Enqueue(container);

            while (q.Count > 0) {
                var c = q.Dequeue();

                if (c.Components.Count == 0) {
                    result = default(T);
                    return false;
                }

                foreach (var element in c.Components) {
                    if (element is T t) {
                        result = t;
                        return true;
                    }

                    if (element is IBaseGameComponentContainer c2) {
                        q.Enqueue(c2);
                    }
                }
            }

            result = default(T);
            return false;
        }

    }
}
