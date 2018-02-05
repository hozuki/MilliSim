using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace OpenMLTD.MilliSim.Foundation.Extensions {
    public static class GameExtensions {

        public static BaseGame ToBaseGame([NotNull] this Game game) {
            return (BaseGame)game;
        }

        [CanBeNull]
        public static T FindSingleElement<T>([NotNull] this Game game) where T : class, IBaseGameComponent {
            foreach (var component in game.Components) {
                if (component is T t) {
                    return t;
                }
            }

            foreach (var component in game.Components) {
                if (!(component is IBaseGameComponentContainer c)) {
                    continue;
                }

                var r = c.FindSingleOrNull<T>();

                if (r != null) {
                    return r;
                }
            }

            return null;
        }

    }
}
