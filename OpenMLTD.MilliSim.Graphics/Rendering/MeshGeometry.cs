using System.Runtime.InteropServices;
using SharpDX;
using SharpDX.Direct3D11;

namespace OpenMLTD.MilliSim.Graphics.Rendering {

    public sealed class MeshGeometry : DisposeBase {

        internal MeshGeometry() {
        }

        internal void SetVertices<TVertex>(Device device, TVertex[] vertices) where TVertex : struct {
            Utilities.Dispose(ref _vertexBuffer);
            _vertexStride = Marshal.SizeOf(typeof(TVertex));

            var vbd = new BufferDescription(
                _vertexStride * vertices.Length,
                ResourceUsage.Immutable,
                BindFlags.VertexBuffer,
                CpuAccessFlags.None,
                ResourceOptionFlags.None,
                0
            );
            _vertexBuffer = new Buffer(device, DataStream.Create(vertices, false, false), vbd);
        }

        internal void SetIndices(Device device, int[] indices) {
            var ibd = new BufferDescription(
                sizeof(int) * indices.Length,
                ResourceUsage.Immutable,
                BindFlags.IndexBuffer,
                CpuAccessFlags.None,
                ResourceOptionFlags.None,
                0
            );
            _indexBuffer = new Buffer(device, DataStream.Create(indices, false, false), ibd);
            _faceCount = indices.Length / 3;
        }

        public void Draw(DeviceContext context) {
            const int offset = 0;
            context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(_vertexBuffer, _vertexStride, offset));
            context.InputAssembler.SetIndexBuffer(_indexBuffer, SharpDX.DXGI.Format.R32_UInt, 0);
            context.DrawIndexed(_faceCount * 3, 0, 0);
        }

        protected override void Dispose(bool disposing) {
            if (!disposing) {
                return;
            }
            Utilities.Dispose(ref _vertexBuffer);
            Utilities.Dispose(ref _indexBuffer);
        }

        private Buffer _vertexBuffer;
        private Buffer _indexBuffer;
        private int _faceCount;
        private int _vertexStride;

    }
}
