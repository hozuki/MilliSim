using System.Collections.Generic;
using OpenMLTD.MilliSim.Graphics.Rendering.Direct3D;
using SharpDX;
using SharpDX.Direct3D11;

namespace OpenMLTD.MilliSim.Graphics.Rendering {
    public abstract class ModelBase<TVertex> : DisposeBase where TVertex : struct {

        protected ModelBase() {
            _vertices = new List<TVertex>();
            _indices = new List<int>();
            Mesh = new MeshGeometry();
        }

        public MeshGeometry Mesh { get; }

        public D3DMaterial Material { get; set; }

        public ShaderResourceView DiffuseMap { get; set; }

        public BoundingBox BoundingBox { get; protected set; }

        protected override void Dispose(bool disposing) {
            if (!disposing) {
                return;
            }
            var meshGeometry = Mesh;
            Utilities.Dispose(ref meshGeometry);
            _indices = null;
            _vertices = null;
        }

        protected IReadOnlyList<int> _indices;

        protected IReadOnlyList<TVertex> _vertices;

    }
}
