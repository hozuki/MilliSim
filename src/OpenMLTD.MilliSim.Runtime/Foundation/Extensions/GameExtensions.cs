using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace OpenMLTD.MilliSim.Foundation.Extensions {
    /// <summary>
    /// Helper functions for <see cref="Game"/>.
    /// </summary>
    public static class GameExtensions {

        /// <summary>
        /// Forces to convert a <see cref="Game"/> to a <see cref="BaseGame"/>.
        /// </summary>
        /// <param name="game">The <see cref="Game"/> to convert.</param>
        /// <returns>Converted <see cref="BaseGame"/>.</returns>
        [NotNull]
        public static BaseGame ToBaseGame([NotNull] this Game game) {
            return (BaseGame)game;
        }

        /// <summary>
        /// Finds the first element of the specified type. If there is no such an element, returns <see langword="null"/>.
        /// </summary>
        /// <typeparam name="T">Type of the element.</typeparam>
        /// <param name="game">The <see cref="Game"/> to start searching from.</param>
        /// <returns>Found element or <see langword="null"/> if there is no match.</returns>
        [CanBeNull]
        public static T FindFirstElementOrDefault<T>([NotNull] this Game game) where T : class, IBaseGameComponent {
            foreach (var component in game.Components) {
                if (component is T t) {
                    return t;
                }
            }

            foreach (var component in game.Components) {
                if (!(component is IBaseGameComponentContainer c)) {
                    continue;
                }

                var r = c.FindFirstElementOrDefault<T>();

                if (r != null) {
                    return r;
                }
            }

            return null;
        }

    }
}
