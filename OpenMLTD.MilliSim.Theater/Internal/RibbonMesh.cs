using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Core.Entities.Runtime;
using OpenMLTD.MilliSim.Graphics;
using OpenMLTD.MilliSim.Theater.Animation;
using OpenMLTD.MilliSim.Theater.Animation.Extending;
using SharpDX;
using SharpDX.Direct3D11;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace OpenMLTD.MilliSim.Theater.Internal {
    internal struct RibbonMesh : IDisposable {

        internal RibbonMesh(Device device, int slice, float textureTopYRatio, float textureBottomYRatio, float z, RibbonParameters[] rps, INoteTraceCalculator traceCalculator, double now, (RuntimeNote Start, RuntimeNote End)[] notePairs, NoteMetrics visualNoteMetrics, NoteAnimationMetrics animationMetrics) {
            _vertexBuffer = null;
            _indexBuffer = null;
            _vertexStride = 0;
            _faceCount = 0;
            _vertices = null;
            _indices = null;
            _vertexDataStream = null;
            _indexDataStream = null;

            SetMeshParameters(device, slice, textureTopYRatio, textureBottomYRatio, z, rps, traceCalculator, now, notePairs, visualNoteMetrics, animationMetrics);
        }

        public void Dispose() {
            Utilities.Dispose(ref _indexBuffer);
            Utilities.Dispose(ref _vertexBuffer);
            Utilities.Dispose(ref _vertexDataStream);
            Utilities.Dispose(ref _indexDataStream);
        }

        private void SetMeshParameters(Device device, int slice, float textureTopYRatio, float textureBottomYRatio, float z, RibbonParameters[] rps, INoteTraceCalculator traceCalculator, double now, (RuntimeNote Start, RuntimeNote End)[] notePairs, NoteMetrics visualNoteMetrics, NoteAnimationMetrics animationMetrics) {
            if (rps == null) {
                return;
            }
            if (notePairs == null) {
                return;
            }

            if (rps.Length == 0 || notePairs.Length == 0 || rps.Length != notePairs.Length) {
                throw new ArgumentException();
            }

            if (!rps.Any(rp => rp.Visible)) {
                throw new ArgumentException();
            }

            Dispose();

            var vertexCount = 0;
            var indexCount = 0;

            var allRibbonsBottomY = rps[0].Y1;
            var allRibbonsTopY = rps[rps.Length - 1].Y2;
#if DEBUG
            if (GlobalDebug.Enabled) {
                Debug.Print("Top: {0}, Bottom: {1}", allRibbonsTopY, allRibbonsBottomY);
            }
#endif

            foreach (var rp in rps) {
                if (!rp.Visible) {
                    continue;
                }

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

            var vertexStart = 0;
            var indexStart = 0;

            // 1---2
            // | / |
            // 3---4
            // (1,2,3) (4,3,2)

            for (var i = 0; i < rps.Length; ++i) {
                var rp = rps[i];

                if (!rp.Visible) {
                    continue;
                }

                var notePair = notePairs[i];

                float perc;
                float v;

                if (rp.IsLine) {
                    var startRadius = traceCalculator.GetNoteRadius(notePair.Start, now, visualNoteMetrics, animationMetrics);
                    var endRadius = traceCalculator.GetNoteRadius(notePair.End, now, visualNoteMetrics, animationMetrics);

                    perc = (rp.Y1 - allRibbonsBottomY) / (allRibbonsTopY - allRibbonsBottomY);
                    v = MathHelper.Lerp(textureTopYRatio, textureBottomYRatio, perc);
                    var leftTopVertex = new MeshVertex(rp.X1 - endRadius.Width / 2, rp.Y1, z, 0, 0, 1, 1, 0, 0, 0, v);
                    var rightTopVertex = new MeshVertex(rp.X1 + endRadius.Width / 2, rp.Y1, z, 0, 0, 1, 1, 0, 0, 1, v);
                    perc = (rp.Y2 - allRibbonsBottomY) / (allRibbonsTopY - allRibbonsBottomY);
                    v = MathHelper.Lerp(textureTopYRatio, textureBottomYRatio, perc);
                    var leftBottomVertex = new MeshVertex(rp.X2 - startRadius.Width / 2, rp.Y2, z, 0, 0, 1, 1, 0, 0, 0, v);
                    var rightBottomVertex = new MeshVertex(rp.X2 + startRadius.Width / 2, rp.Y2, z, 0, 0, 1, 1, 0, 0, 1, v);

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
                    double startTime, endTime;

                    var startStatus = NoteAnimationHelper.GetOnStageStatusOf(notePair.Start, now, animationMetrics);
                    if (startStatus == OnStageStatus.Passed) {
                        startTime = now;
                    } else {
                        startTime = notePair.Start.HitTime;
                    }

                    var endStatus = NoteAnimationHelper.GetOnStageStatusOf(notePair.End, now, animationMetrics);
                    if (endStatus == OnStageStatus.Incoming) {
                        var endTimePoints = NoteAnimationHelper.CalculateNoteTimePoints(notePair.End, animationMetrics);
                        endTime = now + endTimePoints.Duration;
                    } else {
                        endTime = notePair.End.HitTime;
                    }

                    var deltaTime = endTime - startTime;

                    for (var j = 0; j <= slice; ++j) {
                        var t = (float)j / slice;
                        var ribbonTime = endTime - deltaTime * t;
                        var pt = RibbonMathHelper.CubicBezier(rp, t);

                        var noteRadius = traceCalculator.GetNoteRadius(notePair.Start, ribbonTime, visualNoteMetrics, animationMetrics);
                        perc = (pt.Y - allRibbonsBottomY) / (allRibbonsTopY - allRibbonsBottomY);
                        v = MathHelper.Lerp(textureTopYRatio, textureBottomYRatio, perc);
                        var leftVertex = new MeshVertex(pt.X - noteRadius.Width / 2, pt.Y, z, 0, 0, 1, 1, 0, 0, 0, v);
                        var rightVertex = new MeshVertex(pt.X + noteRadius.Width / 2, pt.Y, z, 0, 0, 1, 1, 0, 0, 1, v);

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
