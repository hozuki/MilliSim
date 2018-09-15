using Microsoft.Xna.Framework;

namespace OpenMLTD.MilliSim.Graphics {
    /// <inheritdoc />
    /// <summary>
    /// The interface of <see cref="IVisual" />s on 2D planes.
    /// </summary>
    public interface IVisual2D : IVisual {

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        Vector2 Location { get; set; }

    }
}
