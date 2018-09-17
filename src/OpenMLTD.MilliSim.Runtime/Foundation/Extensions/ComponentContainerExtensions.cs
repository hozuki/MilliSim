using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace OpenMLTD.MilliSim.Foundation.Extensions {
    /// <summary>
    /// Extension functions for <see cref="IBaseGameComponentContainer"/>.
    /// </summary>
    public static class ComponentContainerExtensions {

        /// <summary>
        /// Finds the first element of the specified type. If there is no such an element, returns <see langword="null"/>.
        /// </summary>
        /// <typeparam name="T">Type of the element.</typeparam>
        /// <param name="container">The <see cref="IBaseGameComponentContainer"/> to start searching.</param>
        /// <returns>Found element or <see langword="null"/> if there is no match.</returns>
        [CanBeNull]
        public static T FindFirstElementOrDefault<T>(this IBaseGameComponentContainer container) where T : class, IBaseGameComponent {
            var b = TryFind<T>(container, out var r);
            return b ? r : null;
        }

        /// <summary>
        /// Finds the first element of the specified type. If there is no such an element, throws an <see cref="InvalidOperationException"/>.
        /// </summary>
        /// <typeparam name="T">Type of the element.</typeparam>
        /// <param name="container">The <see cref="IBaseGameComponentContainer"/> to start searching.</param>
        /// <returns>Found element.</returns>
        /// <exception cref="InvalidOperationException">There is no such an element.</exception>
        [NotNull]
        public static T FindFirstElement<T>(this IBaseGameComponentContainer container) where T : IBaseGameComponent {
            var b = TryFind<T>(container, out var r);
            if (!b || r == null) {
                // Similar to Enumerable.First().
                throw new InvalidOperationException();
            }
            return r;
        }

        /// <summary>
        /// Tries to find the first element of the specified type.
        /// </summary>
        /// <typeparam name="T">Type of the element.</typeparam>
        /// <param name="container">The <see cref="IBaseGameComponentContainer"/> to start searching.</param>
        /// <param name="result">Result element.</param>
        /// <returns><see langword="true"/> if there is at least a match, otherwise <see langword="false"/>.</returns>
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
