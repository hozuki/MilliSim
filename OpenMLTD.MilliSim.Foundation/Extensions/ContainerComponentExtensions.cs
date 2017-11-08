using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace OpenMLTD.MilliSim.Foundation.Extensions {
    public static class ContainerComponentExtensions {

        [CanBeNull]
        public static T FindOrNull<T>(this IComponentContainer container) where T : class, IComponent {
            var b = TryFind<T>(container, out var r);
            return b ? r : null;
        }

        [NotNull]
        public static T Find<T>(this IComponentContainer container) where T : IComponent {
            var b = TryFind<T>(container, out var r);
            if (!b) {
                // Similar to Enumerable.Single().
                throw new InvalidOperationException();
            }
            return r;
        }

        public static bool TryFind<T>(this IComponentContainer container, [CanBeNull] out T result) where T : IComponent {
            var q = new Queue<IComponentContainer>();
            q.Enqueue(container);

            while (q.Count > 0) {
                var c = q.Dequeue();

                if (c.Components.Count == 0) {
                    result = default;
                    return false;
                }

                foreach (var element in c.Components) {
                    if (element is T t) {
                        result = t;
                        return true;
                    }
                    if (element is IComponentContainer c2) {
                        q.Enqueue(c2);
                    }
                }
            }

            result = default;
            return false;
        }

        public static Component CreateAndAdd([NotNull] this IComponentContainer container, [NotNull] Type type) {
            return Component.CreateAndAddTo(container, type);
        }

        public static T CreateAndAdd<T>([NotNull] this IComponentContainer container) where T : Component {
            return Component.CreateAndAddTo<T>(container);
        }

    }
}
