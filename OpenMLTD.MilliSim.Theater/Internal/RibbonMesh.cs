using System;
using System.Runtime.InteropServices;
using OpenMLTD.MilliSim.Graphics;
using OpenMLTD.MilliSim.Theater.Animation;
using SharpDX;
using SharpDX.Direct3D11;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace OpenMLTD.MilliSim.Theater.Internal {
    internal struct RibbonMesh : IDisposable {

        internal RibbonMesh(Device device, int slice, float width, float z, params RibbonParameters[] rps) {
            _vertexBuffer = null;
            _indexBuffer = null;
            _vertexStride = 0;
            _faceCount = 0;
            _vertices = null;
            _indices = null;
            _vertexDataStream = null;
            _indexDataStream = null;

            SetMeshParameters(device, slice, width, z, rps);
        }

        public void Dispose() {
            Utilities.Dispose(ref _indexBuffer);
            Utilities.Dispose(ref _vertexBuffer);
            Utilities.Dispose(ref _vertexDataStream);
            Utilities.Dispose(ref _indexDataStream);
        }

        internal void SetMeshParameters(Device device, int slice, float width, float z, params RibbonParameters[] rps) {
            Dispose();

            if (rps == null || rps.Length == 0) {
                return;
            }

            var halfWidth = width / 2;

            var vertexCount = 0;
            var indexCount = 0;

            var startY = rps[0].Y1;
            var endY = rps[rps.Length - 1].Y2;

            foreach (var rp in rps) {
                if (rp.IsLine) {
                    vertexCount += 4;
                    indexCount += 6;
                } else {
                    vertexCount += (slice + 1) * 2;
                    indexCount += slice * 6;
                }
            }

            var vertices = new MeshVertex[vertexCount];
            var indices = new int[indexCount];
            var halfVertexCount = vertexCount / 2;

            var vertexStart = 0;
            var indexStart = 0;

            // 1---2
            // | / |
            // 3---4
            // (1,2,3) (4,3,2)

            foreach (var rp in rps) {
                if (rp.IsLine) {
                    var leftTopVertex = new MeshVertex(rp.X1 - halfWidth, rp.Y1, z, 0, 0, 1, 1, 0, 0, 0, (rp.Y1 - startY) / (endY - startY));
                    var rightTopVertex = new MeshVertex(rp.X1 + halfWidth, rp.Y1, z, 0, 0, 1, 1, 0, 0, 1, (rp.Y1 - startY) / (endY - startY));
                    var leftBottomVertex = new MeshVertex(rp.X2 - halfWidth, rp.Y2, z, 0, 0, 1, 1, 0, 0, 0, (rp.Y2 - startY) / (endY - startY));
                    var rightBottomVertex = new MeshVertex(rp.X2 + halfWidth, rp.Y2, z, 0, 0, 1, 1, 0, 0, 1, (rp.Y2 - startY) / (endY - startY));

                    vertices[vertexStart] = leftTopVertex;
                    vertices[vertexStart + 1] = rightTopVertex;
                    vertices[vertexStart + 2] = leftBottomVertex;
                    vertices[vertexStart + 3] = rightBottomVertex;

                    indices[indexStart] = vertexStart;
                    indices[indexStart + 1] = vertexStart + 1;
                    indices[indexStart + 2] = vertexStart + 2;
                    indices[indexStart + 3] = vertexStart + 3;
                    indices[indexStart + 4] = vertexStart + 2;
                    indices[indexStart + 5] = vertexStart + 1;

                    vertexStart += 4;
                    indexStart += 6;
                } else {
                    for (var j = 0; j <= slice; ++j) {
                        var t = (float)j / slice;
                        var pt = RibbonMathHelper.CubicBezier(rp, t);

                        var leftVertex = new MeshVertex(pt.X - halfWidth, pt.Y, z, 0, 0, 1, 1, 0, 0, 0, (pt.Y - startY) / (endY - startY));
                        var rightVertex = new MeshVertex(pt.X + halfWidth, pt.Y, z, 0, 0, 1, 1, 0, 0, 1, (pt.Y - startY) / (endY - startY));

                        vertices[vertexStart + j * 2] = leftVertex;
                        vertices[vertexStart + j * 2 + 1] = rightVertex;
                    }

                    for (var j = 0; j < slice; ++j) {
                        indices[indexStart + j * 6] = vertexStart + j * 2;
                        indices[indexStart + j * 6 + 1] = vertexStart + j * 2 + 1;
                        indices[indexStart + j * 6 + 2] = vertexStart + j * 2 + 2;
                        indices[indexStart + j * 6 + 3] = vertexStart + j * 2 + 3;
                        indices[indexStart + j * 6 + 4] = vertexStart + j * 2 + 2;
                        indices[indexStart + j * 6 + 5] = vertexStart + j * 2 + 1;
                    }

                    vertexStart += (slice + 1) * 2;
                    indexStart += slice * 6;
                }
            }

            _vertices = vertices;
            _indices = indices;

            SetVertices(device, vertices);
            SetIndices(device, indices);
        }

        internal void Draw(DeviceContext deviceContext) {
            const int offset = 0;
            deviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(_vertexBuffer, _vertexStride, offset));
            deviceContext.InputAssembler.SetIndexBuffer(_indexBuffer, SharpDX.DXGI.Format.R32_UInt, 0);
            deviceContext.DrawIndexed(_faceCount * 3, 0, 0);
        }

        private void SetVertices<TVertex>(Device device, TVertex[] vertices) where TVertex : struct {
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
            var dataStream = DataStream.Create(vertices, false, false);
            _vertexBuffer = new Buffer(device, dataStream, vbd);
            _vertexDataStream = dataStream;
        }

        private void SetIndices(Device device, int[] indices) {
            var ibd = new BufferDescription(
                sizeof(int) * indices.Length,
                ResourceUsage.Immutable,
                BindFlags.IndexBuffer,
                CpuAccessFlags.None,
                ResourceOptionFlags.None,
                0
            );
            var dataStream = DataStream.Create(indices, false, false);
            _indexBuffer = new Buffer(device, dataStream, ibd);
            _faceCount = indices.Length / 3;
            _indexDataStream = dataStream;
        }

        private MeshVertex[] _vertices;
        private int[] _indices;
        private DataStream _vertexDataStream;
        private DataStream _indexDataStream;
        private Buffer _vertexBuffer;
        private Buffer _indexBuffer;
        private int _faceCount;
        private int _vertexStride;

    }
}
