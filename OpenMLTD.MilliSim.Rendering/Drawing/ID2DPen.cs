using System;
using SharpDX.Direct2D1;

namespace OpenMLTD.MilliSim.Rendering.Drawing {
    public interface ID2DPen : IDisposable {

        StrokeStyle StrokeStyle { get; }

        ID2DBrush Brush { get; }

        float StrokeWidth { get; }

    }
}
