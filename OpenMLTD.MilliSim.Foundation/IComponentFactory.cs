using System;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Core;

namespace OpenMLTD.MilliSim.Foundation {
    public interface IComponentFactory : IMilliSimPlugin, IDisposable {

        /// <summary>
        /// Creates game component.
        /// </summary>
        /// <param name="game"></param>
        /// <param name="parent">The parent of created <see cref="IComponent"/>.</param>
        /// <returns>A <see cref="IComponent"/>. If this function returns <see langword="null"/>, it means the factory is not going to create an <see cref="IComponent"/>.</returns>
        [CanBeNull]
        IComponent CreateComponent([NotNull] GameBase game, [NotNull] IComponentContainer parent);

    }
}
