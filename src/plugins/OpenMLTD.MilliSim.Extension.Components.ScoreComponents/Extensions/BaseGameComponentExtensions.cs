using System;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Foundation;

namespace OpenMLTD.MilliSim.Extension.Components.ScoreComponents.Extensions {
    internal static class BaseGameComponentExtensions {

        internal static BaseGameComponent CreateAndAdd([NotNull] this IBaseGameComponentContainer container, [NotNull] BaseGame game, [NotNull] Type type) {
            return BaseGameComponent.CreateAndAddTo(game, container, type);
        }

        internal static T CreateAndAdd<T>([NotNull] this IBaseGameComponentContainer container, [NotNull] BaseGame game) where T : BaseGameComponent {
            return BaseGameComponent.CreateAndAddTo<T>(game, container);
        }

    }
}
