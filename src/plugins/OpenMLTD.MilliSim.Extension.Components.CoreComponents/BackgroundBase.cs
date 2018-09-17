using JetBrains.Annotations;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Graphics;

namespace OpenMLTD.MilliSim.Extension.Components.CoreComponents {
    /// <inheritdoc />
    /// <summary>
    /// Base background visual element.
    /// </summary>
    public abstract class BackgroundBase : Visual {

        /// <inheritdoc />
        /// <summary>
        /// Creates a new <see cref="T:OpenMLTD.MilliSim.Extension.Components.CoreComponents.BackgroundBase" /> instance.
        /// </summary>
        /// <param name="game">The base game.</param>
        /// <param name="parent">The parent of the <see cref="T:OpenMLTD.MilliSim.Extension.Components.CoreComponents.BackgroundBase" />.</param>
        protected BackgroundBase([NotNull] BaseGame game, [NotNull] IVisualContainer parent)
            : base(game, parent) {
        }

    }
}
