using System;
using SharpDX.Direct2D1;

namespace OpenMLTD.MilliSim.Rendering.Drawing {
    public interface ID2DImage : IDisposable {

        Image NativeImage { get; }

    }
}
