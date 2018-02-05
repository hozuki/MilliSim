using System;
using System.Drawing;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpenMLTD.MilliSim.Contributed.Scores.Animation;

namespace OpenMLTD.MilliSim.Extension.Components.ScoreComponents.Internal {
    internal struct RibbonMesh : IDisposable {

        internal RibbonMesh(RibbonMeshCreateParams createParams) {
            _vertexStride = 0;
            _faceCount = 0;
            _vertices = null;
            _indices = null;
            _vertexBuffer = null;
            _indexBuffer = null;

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
                    float startTime, endTime;

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

                    var pts = new Vector2[slice + 1];

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
                // An empty ribbon.

                SetVertices(createParams.Device, (RibbonVertex[])null);
                SetIndices(createParams.Device, null);
            } else {
                // Normal case.

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

                var vertices = new RibbonVertex[vertexCount];
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

                var traceCalculator = createParams.AnimationCalculator;
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
                        var leftTopVertex = new RibbonVertex(rp.X1 - endRadius.X / 2, rp.Y1, zToUse, 0, 0, 1, 0, v);
                        var rightTopVertex = new RibbonVertex(rp.X1 + endRadius.X / 2, rp.Y1, zToUse, 0, 0, 1, 1, v);
                        perc = (processedRibbonLength + Math.Abs(rp.Y1 - rp.Y2)) / ribbonLength;
                        v = MathHelper.Lerp(textureTopYRatio, textureBottomYRatio, perc);
                        var leftBottomVertex = new RibbonVertex(rp.X2 - startRadius.X / 2, rp.Y2, zToUse, 0, 0, 1, 0, v);
                        var rightBottomVertex = new RibbonVertex(rp.X2 + startRadius.X / 2, rp.Y2, zToUse, 0, 0, 1, 1, v);

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
                            var leftVertex = new RibbonVertex(pt.X - noteRadius.X / 2, pt.Y, zToUse, 0, 0, 1, 0, v);
                            var rightVertex = new RibbonVertex(pt.X + noteRadius.X / 2, pt.Y, zToUse, 0, 0, 1, 1, v);

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
        }

        public void Dispose() {
            _vertexBuffer?.Dispose();
            _indexBuffer?.Dispose();
            _vertexBuffer = null;
            _indexBuffer = null;
        }

        internal void Draw([NotNull] GraphicsDevice graphicsDevice) {
            if (_vertexBuffer == null || _indexBuffer == null) {
                return;
            }

            graphicsDevice.SetVertexBuffer(_vertexBuffer);
            graphicsDevice.Indices = _indexBuffer;
            graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, _faceCount);
        }

        private void SetVertices<TVertex>([NotNull] GraphicsDevice graphicsDevice, [CanBeNull] TVertex[] vertices) where TVertex : struct {
            _vertexBuffer?.Dispose();

            if (vertices == null || vertices.Length == 0) {
                return;
            }

            var vertexDeclaration = new VertexDeclaration(RibbonMeshVertexElements);
            var vertexBuffer = new VertexBuffer(graphicsDevice, vertexDeclaration, vertices.Length, BufferUsage.WriteOnly);

            vertexBuffer.SetData(vertices);

            _vertexBuffer = vertexBuffer;
        }

        private void SetIndices([NotNull] GraphicsDevice graphicsDevice, [CanBeNull] ushort[] indices) {
            _indexBuffer?.Dispose();

            if (indices == null || indices.Length == 0) {
                return;
            }

            var indexBuffer = new IndexBuffer(graphicsDevice, IndexElementSize.SixteenBits, indices.Length, BufferUsage.WriteOnly);

            indexBuffer.SetData(indices);

            _indexBuffer = indexBuffer;
            _faceCount = indices.Length / 3;
        }

        [CanBeNull]
        private RibbonVertex[] _vertices;
        [CanBeNull]
        private ushort[] _indices;
        [CanBeNull]
        private VertexBuffer _vertexBuffer;
        [CanBeNull]
        private IndexBuffer _indexBuffer;
        private int _faceCount;
        private int _vertexStride;

        private static readonly VertexElement[] RibbonMeshVertexElements = {
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
            new VertexElement(24, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
        };

        private struct RibbonPointCache {

            internal float StartTime { get; set; }

            internal float EndTime { get; set; }

            // length = 'slice' arg value + 1
            internal Vector2[] Locations { get; set; }

        }

    }
}
