using System;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Core;

namespace OpenMLTD.MilliSim.Foundation.Extending {
    /// <inheritdoc cref="IMilliSimPlugin"/>
    /// <inheritdoc cref="IDisposable"/>
    /// <summary>
    /// The interface for factories of base game components.
    /// </summary>
    public interface IBaseGameComponentFactory : IMilliSimPlugin, IDisposable {

        /// <summary>
        /// Creates game component.
        /// </summary>
        /// <param name="game"></param>
        /// <param name="parent">The parent of created <see cref="IBaseGameComponent"/>.</param>
        /// <returns>A <see cref="IBaseGameComponent"/>. If this function returns <see langword="null"/>, it means the factory is not going to create an <see cref="IBaseGameComponent"/>.</returns>
        [CanBeNull]
        IBaseGameComponent CreateComponent([NotNull] BaseGame game, [NotNull] IBaseGameComponentContainer parent);

    }
}
