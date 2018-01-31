using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Graphics;
using OpenMLTD.MilliSim.Theater.Animation;
using SharpDX;
using SharpDX.Direct3D11;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace OpenMLTD.MilliSim.Extension.Components.ScoreComponents.Internal {
    internal struct RibbonMesh : IDisposable {

        internal RibbonMesh(RibbonMeshCreateParams createParams) {
            _vertexBuffer = null;
            _indexBuffer = null;
            _vertexStride = 0;
            _faceCount = 0;
            _vertices = null;
            _indices = null;
            _vertexDataStream = null;
            _indexDataStream = null;

            if (createParams.RibbonParameters == null) {
                return;
            }
            if (createParams.NotePairs == null) {
                return;
            }

            if (createParams.RibbonParameters.Length == 0 || createParams.NotePairs.Length == 0 || createParams.RibbonParameters.Length != createParams.NotePairs.Length) {
                throw new ArgumentException();
            }

            if (!createParams.RibbonParameters.Any(rp => rp.Visible)) {
                throw new ArgumentException();
            }

            Dispose();

            var vertexCount = 0;
            var indexCount = 0;

            var now = createParams.Now;
            var slice = createParams.Slice;
            var visualNoteMetrics = createParams.VisualNoteMetrics;
            var animationMetrics = createParams.AnimationMetrics;

            // First, calculate the total ribbon length.
            // Since CGSS allows ribbon to "fold", we cannot simply use Y coord value to calculate texture V value.
            var bezierCount = createParams.RibbonParameters.Count(rp => rp.Visible && !rp.IsLine);
            var ribbonCache = bezierCount > 0 ? new RibbonPointCache[bezierCount] : null;
            var ribbonLength = 0f;
            var bezierIndex = 0;
            for (var i = 0; i < createParams.RibbonParameters.Length; ++i) {
                var rp = createParams.RibbonParameters[i];

                if (!rp.Visible) {
                    continue;
                }

                var notePair = createParams.NotePairs[i];

                if (rp.IsLine) {
                    ribbonLength += Math.Abs(rp.Y1 - rp.Y2);
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

                    var pts = new PointF[slice + 1];

                    for (var j = 0; j <= slice; ++j) {
                        var t = (float)j / slice;
                        var pt = RibbonMathHelper.CubicBezier(rp, t);
                        pts[j] = pt;

                        if (j > 0) {
                            ribbonLength += Math.Abs(pt.Y - pts[j - 1].Y);
                        }
                    }

                    var pointCache = new RibbonPointCache {
                        StartTime = startTime,
                        EndTime = endTime,
                        Locations = pts
                    };
                    if (ribbonCache == null) {
                        throw new InvalidOperationException();
                    }
                    ribbonCache[bezierIndex] = pointCache;
                    ++bezierIndex;
                }
            }

            if (ribbonLength <= 0) {
                throw new InvalidOperationException();
            }

            foreach (var rp in createParams.RibbonParameters) {
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

            var z = createParams.Z;
            var foldedZ = z - createParams.LayerDepth / 2;

            var vertices = new MeshVertex[vertexCount];
            var indices = new ushort[indexCount];

            var vertexStart = 0;
            var indexStart = 0;

            // Generated vertex order for a non-folded fragment:
            // 1---2
            // | / |
            // 3---4
            // (1,2,3) (4,3,2)

            var processedRibbonLength = 0f;
            bezierIndex = 0;

            var traceCalculator = createParams.TraceCalculator;
            var textureTopYRatio = createParams.TextureTopYRatio;
            var textureBottomYRatio = createParams.TextureBottomYRatio;

            for (var i = 0; i < createParams.RibbonParameters.Length; ++i) {
                var rp = createParams.RibbonParameters[i];

                if (!rp.Visible) {
                    continue;
                }

                var notePair = createParams.NotePairs[i];

                // Normal ribbon is flowing from top to bottom (Y1 < Y2); CGSS would have a part from downside to upside.
                // If this fragment is folded, we must place it under the normal layer (z - layerDepth / 2).
                bool isFragmentFolded;
                float zToUse;

                float perc;
                float v;

                if (rp.IsLine) {
                    var startRadius = traceCalculator.GetNoteRadius(notePair.Start, now, visualNoteMetrics, animationMetrics);
                    var endRadius = traceCalculator.GetNoteRadius(notePair.End, now, visualNoteMetrics, animationMetrics);

                    isFragmentFolded = rp.Y1 > rp.Y2;
                    zToUse = isFragmentFolded ? foldedZ : z;

                    perc = processedRibbonLength / ribbonLength;
                    v = MathHelper.Lerp(textureTopYRatio, textureBottomYRatio, perc);
                    var leftTopVertex = new MeshVertex(rp.X1 - endRadius.Width / 2, rp.Y1, zToUse, 0, 0, 1, 1, 0, 0, 0, v);
                    var rightTopVertex = new MeshVertex(rp.X1 + endRadius.Width / 2, rp.Y1, zToUse, 0, 0, 1, 1, 0, 0, 1, v);
                    perc = (processedRibbonLength + Math.Abs(rp.Y1 - rp.Y2)) / ribbonLength;
                    v = MathHelper.Lerp(textureTopYRatio, textureBottomYRatio, perc);
                    var leftBottomVertex = new MeshVertex(rp.X2 - startRadius.Width / 2, rp.Y2, zToUse, 0, 0, 1, 1, 0, 0, 0, v);
                    var rightBottomVertex = new MeshVertex(rp.X2 + startRadius.Width / 2, rp.Y2, zToUse, 0, 0, 1, 1, 0, 0, 1, v);

                    vertices[vertexStart] = leftTopVertex;
                    vertices[vertexStart + 1] = rightTopVertex;
                    vertices[vertexStart + 2] = leftBottomVertex;
                    vertices[vertexStart + 3] = rightBottomVertex;

                    if (isFragmentFolded) {
                        indices[indexStart] = (ushort)(vertexStart + 2);
                        indices[indexStart + 1] = (ushort)(vertexStart + 1);
                        indices[indexStart + 2] = (ushort)vertexStart;
                        indices[indexStart + 3] = (ushort)(vertexStart + 1);
                        indices[indexStart + 4] = (ushort)(vertexStart + 2);
                        indices[indexStart + 5] = (ushort)(vertexStart + 3);
                    } else {
                        indices[indexStart] = (ushort)vertexStart;
                        indices[indexStart + 1] = (ushort)(vertexStart + 1);
                        indices[indexStart + 2] = (ushort)(vertexStart + 2);
                        indices[indexStart + 3] = (ushort)(vertexStart + 3);
                        indices[indexStart + 4] = (ushort)(vertexStart + 2);
                        indices[indexStart + 5] = (ushort)(vertexStart + 1);
                    }

                    vertexStart += 4;
                    indexStart += 6;
                    processedRibbonLength += Math.Abs(rp.Y1 - rp.Y2);
                } else {
                    if (ribbonCache == null) {
                        throw new InvalidOperationException();
                    }

                    var cache = ribbonCache[bezierIndex];
                    var deltaTime = cache.EndTime - cache.StartTime;

                    for (var j = 0; j <= slice; ++j) {
                        var t = (float)j / slice;
                        var ribbonTime = cache.EndTime - deltaTime * t;
                        var pt = cache.Locations[j];

                        if (j < slice) {
                            isFragmentFolded = pt.Y > cache.Locations[j + 1].Y;
                        } else {
                            // Here, j = slice
                            isFragmentFolded = cache.Locations[slice - 1].Y > pt.Y;
                        }
                        zToUse = isFragmentFolded ? foldedZ : z;

                        var noteRadius = traceCalculator.GetNoteRadius(notePair.Start, ribbonTime, visualNoteMetrics, animationMetrics);
                        perc = processedRibbonLength / ribbonLength;
                        v = MathHelper.Lerp(textureTopYRatio, textureBottomYRatio, perc);
                        var leftVertex = new MeshVertex(pt.X - noteRadius.Width / 2, pt.Y, zToUse, 0, 0, 1, 1, 0, 0, 0, v);
                        var rightVertex = new MeshVertex(pt.X + noteRadius.Width / 2, pt.Y, zToUse, 0, 0, 1, 1, 0, 0, 1, v);

                        vertices[vertexStart + j * 2] = leftVertex;
                        vertices[vertexStart + j * 2 + 1] = rightVertex;

                        if (j < slice) {
                            if (isFragmentFolded) {
                                indices[indexStart + j * 6] = (ushort)(vertexStart + j * 2 + 2);
                                indices[indexStart + j * 6 + 1] = (ushort)(vertexStart + j * 2 + 1);
                                indices[indexStart + j * 6 + 2] = (ushort)(vertexStart + j * 2);
                                indices[indexStart + j * 6 + 3] = (ushort)(vertexStart + j * 2 + 1);
                                indices[indexStart + j * 6 + 4] = (ushort)(vertexStart + j * 2 + 2);
                                indices[indexStart + j * 6 + 5] = (ushort)(vertexStart + j * 2 + 3);
                            } else {
                                indices[indexStart + j * 6] = (ushort)(vertexStart + j * 2);
                                indices[indexStart + j * 6 + 1] = (ushort)(vertexStart + j * 2 + 1);
                                indices[indexStart + j * 6 + 2] = (ushort)(vertexStart + j * 2 + 2);
                                indices[indexStart + j * 6 + 3] = (ushort)(vertexStart + j * 2 + 3);
                                indices[indexStart + j * 6 + 4] = (ushort)(vertexStart + j * 2 + 2);
                                indices[indexStart + j * 6 + 5] = (ushort)(vertexStart + j * 2 + 1);
                            }

                            processedRibbonLength += Math.Abs(cache.Locations[j + 1].Y - cache.Locations[j].Y);
                        }
                    }

                    vertexStart += (slice + 1) * 2;
                    indexStart += slice * 6;
                    ++bezierIndex;
                }
            }

            _vertices = vertices;
            _indices = indices;

            SetVertices(createParams.Device, vertices);
            SetIndices(createParams.Device, indices);
        }

        public void Dispose() {
            Utilities.Dispose(ref _indexBuffer);
            Utilities.Dispose(ref _vertexBuffer);
            Utilities.Dispose(ref _vertexDataStream);
            Utilities.Dispose(ref _indexDataStream);
        }

        internal void Draw(DeviceContext deviceContext) {
            const int offset = 0;
            deviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(_vertexBuffer, _vertexStride, offset));
            deviceContext.InputAssembler.SetIndexBuffer(_indexBuffer, SharpDX.DXGI.Format.R16_UInt, 0);
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

        private void SetIndices(Device device, ushort[] indices) {
            var ibd = new BufferDescription(
                sizeof(ushort) * indices.Length,
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
        private ushort[] _indices;
        private DataStream _vertexDataStream;
        private DataStream _indexDataStream;
        private Buffer _vertexBuffer;
        private Buffer _indexBuffer;
        private int _faceCount;
        private int _vertexStride;

        private struct RibbonPointCache {

            internal double StartTime { get; set; }

            internal double EndTime { get; set; }

            // length = 'slice' arg value + 1
            internal PointF[] Locations { get; set; }

        }

    }
}
