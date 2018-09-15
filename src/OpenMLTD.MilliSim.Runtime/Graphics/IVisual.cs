using Microsoft.Xna.Framework;
using OpenMLTD.MilliSim.Foundation;

namespace OpenMLTD.MilliSim.Graphics {
    /// <inheritdoc cref="IBaseGameComponent"/>
    /// <inheritdoc cref="IDrawable"/>
    /// <summary>
    /// The common interface for drawable game components.
    /// </summary>
    public interface IVisual : IBaseGameComponent, IDrawable {
    }
}
