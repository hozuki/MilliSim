using System;
using SharpDX.Direct2D1;

namespace OpenMLTD.MilliSim.Graphics.Drawing.Direct2D {
    public interface ID2DPen : IDisposable {

        StrokeStyle StrokeStyle { get; }

        ID2DBrush Brush { get; }

        float StrokeWidth { get; }

    }
}
