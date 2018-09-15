using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using OpenMLTD.MilliSim.Foundation;

namespace OpenMLTD.MilliSim.Graphics {
    /// <inheritdoc cref="IVisual"/>
    /// <inheritdoc cref="IBaseGameComponentContainer"/>
    /// <summary>
    /// An interface for <see cref="IVisual"/>s that also serve as <see cref="IBaseGameComponentContainer"/>s.
    /// </summary>
    public interface IVisualContainer : IVisual, IBaseGameComponentContainer {

        /// <summary>
        /// Called before drawing children.
        /// </summary>
        /// <param name="gameTime">Current time.</param>
        void OnBeforeChildrenDraw([NotNull] GameTime gameTime);

        /// <summary>
        /// Called after drawing children.
        /// </summary>
        /// <param name="gameTime">Current time.</param>
        void OnAfterChildrenDraw([NotNull] GameTime gameTime);

    }
}
