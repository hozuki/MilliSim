using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OpenMLTD.MilliSim.Graphics {
    public interface IBufferedVisual : IVisual2D, ISupportsOpacity {

        RenderTarget2D BufferTarget { get; }

        Matrix? Transform { get; set; }

    }
}
