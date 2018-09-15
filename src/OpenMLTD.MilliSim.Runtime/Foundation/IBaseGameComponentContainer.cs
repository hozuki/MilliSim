using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace OpenMLTD.MilliSim.Foundation {
    /// <inheritdoc />
    /// <summary>
    /// An interface for base game component containers.
    /// </summary>
    public interface IBaseGameComponentContainer : IBaseGameComponent {

        /// <summary>
        /// Gets the components in this container.
        /// </summary>
        [NotNull, ItemNotNull]
        BaseGameComponentCollection Components { get; }

        /// <summary>
        /// Called before children are updated.
        /// </summary>
        /// <param name="gameTime">Current time.</param>
        void OnBeforeChildrenUpdate([NotNull] GameTime gameTime);

        /// <summary>
        /// Called after children are updated.
        /// </summary>
        /// <param name="gameTime">Current time.</param>
        void OnAfterChildrenUpdate([NotNull] GameTime gameTime);

    }
}
