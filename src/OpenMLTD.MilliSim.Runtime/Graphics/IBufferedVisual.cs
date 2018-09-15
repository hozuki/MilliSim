using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OpenMLTD.MilliSim.Graphics {
    /// <inheritdoc cref="IVisual2D"/>
    /// <inheritdoc cref="ISupportsOpacity"/>
    /// <summary>
    /// The interface for <see cref="IVisual"/>s that support render buffers for postprocessing.
    /// </summary>
    public interface IBufferedVisual : IVisual2D, ISupportsOpacity {

        /// <summary>
        /// Gets the buffer target.
        /// </summary>
        RenderTarget2D BufferTarget { get; }

        /// <summary>
        /// Gets or sets the transform.
        /// A <see langword="null"/> value is equal to the identity transform.
        /// </summary>
        [CanBeNull]
        Matrix? Transform { get; set; }

    }
}
