using OpenMLTD.MilliSim.Core;
using SharpDX.Direct2D1;

namespace OpenMLTD.MilliSim.Graphics.Drawing.Direct2D {
    public sealed class D2DMesh : DisposableBase {

        public D2DMesh(RenderContext context) {
            _mesh = new Mesh(context.RenderTarget.DeviceContext);
        }

        public D2DMesh(RenderContext context, D2DTriangle[] triangles) {
            var t = new Triangle[triangles.Length];
            for (var i = 0; i < triangles.Length; ++i) {
                t[i] = triangles[i].ToNative();
            }
            _mesh = new Mesh(context.RenderTarget.DeviceContext, t);
        }

        public Mesh Native => _mesh;

        protected override void Dispose(bool disposing) {
            if (disposing) {
                _mesh?.Dispose();
                _mesh = null;
            }
        }

        private Mesh _mesh;

    }
}
