using System.Collections.Generic;
using SharpDX;

namespace OpenMLTD.MilliSim.Graphics {
    public static class GeometryGenerator {

        public static MeshData CreateBox(float width, float height, float depth) {
            var w2 = 0.5f * width;
            var h2 = 0.5f * height;
            var d2 = 0.5f * depth;

            var vertices = new List<MeshVertex>();
            var indices = new List<int>();

            // front
            vertices.Add(new MeshVertex(-w2, -h2, -d2, 0, 0, -1, 1, 0, 0, 0, 1));
            vertices.Add(new MeshVertex(-w2, +h2, -d2, 0, 0, -1, 1, 0, 0, 0, 0));
            vertices.Add(new MeshVertex(+w2, +h2, -d2, 0, 0, -1, 1, 0, 0, 1, 0));
            vertices.Add(new MeshVertex(+w2, -h2, -d2, 0, 0, -1, 1, 0, 0, 1, 1));
            // back
            vertices.Add(new MeshVertex(-w2, -h2, +d2, 0, 0, 1, -1, 0, 0, 1, 1));
            vertices.Add(new MeshVertex(+w2, -h2, +d2, 0, 0, 1, -1, 0, 0, 0, 1));
            vertices.Add(new MeshVertex(+w2, +h2, +d2, 0, 0, 1, -1, 0, 0, 0, 0));
            vertices.Add(new MeshVertex(-w2, +h2, +d2, 0, 0, 1, -1, 0, 0, 1, 0));
            // top
            vertices.Add(new MeshVertex(-w2, +h2, -d2, 0, 1, 0, 1, 0, 0, 0, 1));
            vertices.Add(new MeshVertex(-w2, +h2, +d2, 0, 1, 0, 1, 0, 0, 0, 0));
            vertices.Add(new MeshVertex(+w2, +h2, +d2, 0, 1, 0, 1, 0, 0, 1, 0));
            vertices.Add(new MeshVertex(+w2, +h2, -d2, 0, 1, 0, 1, 0, 0, 1, 1));
            // bottom
            vertices.Add(new MeshVertex(-w2, -h2, -d2, 0, -1, 0, -1, 0, 0, 1, 1));
            vertices.Add(new MeshVertex(+w2, -h2, -d2, 0, -1, 0, -1, 0, 0, 0, 1));
            vertices.Add(new MeshVertex(+w2, -h2, +d2, 0, -1, 0, -1, 0, 0, 0, 0));
            vertices.Add(new MeshVertex(-w2, -h2, +d2, 0, -1, 0, -1, 0, 0, 1, 0));
            // left
            vertices.Add(new MeshVertex(-w2, -h2, +d2, -1, 0, 0, 0, 0, -1, 0, 1));
            vertices.Add(new MeshVertex(-w2, +h2, +d2, -1, 0, 0, 0, 0, -1, 0, 0));
            vertices.Add(new MeshVertex(-w2, +h2, -d2, -1, 0, 0, 0, 0, -1, 1, 0));
            vertices.Add(new MeshVertex(-w2, -h2, -d2, -1, 0, 0, 0, 0, -1, 1, 1));
            // right
            vertices.Add(new MeshVertex(+w2, -h2, -d2, 1, 0, 0, 0, 0, 1, 0, 1));
            vertices.Add(new MeshVertex(+w2, +h2, -d2, 1, 0, 0, 0, 0, 1, 0, 0));
            vertices.Add(new MeshVertex(+w2, +h2, +d2, 1, 0, 0, 0, 0, 1, 1, 0));
            vertices.Add(new MeshVertex(+w2, -h2, +d2, 1, 0, 0, 0, 0, 1, 1, 1));

            indices.AddRange(new[]{
                0,1,2,0,2,3,
                4,5,6,4,6,7,
                8,9,10,8,10,11,
                12,13,14,12,14,15,
                16,17,18,16,18,19,
                20,21,22,20,22,23
            });

            return new MeshData(vertices, indices);
        }

        public static MeshData CreateSphere(float radius, int sliceCount, int stackCount) {
            var vertices = new List<MeshVertex>();

            vertices.Add(new MeshVertex(0, radius, 0, 0, 1, 0, 1, 0, 0, 0, 0));
            var phiStep = MathF.PI / stackCount;
            var thetaStep = 2.0f * MathF.PI / sliceCount;

            for (var i = 1; i <= stackCount - 1; i++) {
                var phi = i * phiStep;
                for (var j = 0; j <= sliceCount; j++) {
                    var theta = j * thetaStep;
                    var p = new Vector3(
                        (radius * MathF.Sin(phi) * MathF.Cos(theta)),
                        (radius * MathF.Cos(phi)),
                        (radius * MathF.Sin(phi) * MathF.Sin(theta)));
                    var t = new Vector3(-radius * MathF.Sin(phi) * MathF.Sin(theta), 0, radius * MathF.Sin(phi) * MathF.Cos(theta));
                    t.Normalize();
                    var n = Vector3.Normalize(p);

                    var uv = new Vector2(theta / (MathF.PI * 2), phi / MathF.PI);
                    vertices.Add(new MeshVertex(p, n, t, uv));
                }
            }
            vertices.Add(new MeshVertex(0, -radius, 0, 0, -1, 0, 1, 0, 0, 0, 1));

            var indices = new List<int>();

            for (var i = 1; i <= sliceCount; i++) {
                indices.Add(0);
                indices.Add(i + 1);
                indices.Add(i);
            }

            var baseIndex = 1;
            var ringVertexCount = sliceCount + 1;
            for (var i = 0; i < stackCount - 2; i++) {
                for (var j = 0; j < sliceCount; j++) {
                    indices.Add(baseIndex + i * ringVertexCount + j);
                    indices.Add(baseIndex + i * ringVertexCount + j + 1);
                    indices.Add(baseIndex + (i + 1) * ringVertexCount + j);

                    indices.Add(baseIndex + (i + 1) * ringVertexCount + j);
                    indices.Add(baseIndex + i * ringVertexCount + j + 1);
                    indices.Add(baseIndex + (i + 1) * ringVertexCount + j + 1);
                }
            }

            var southPoleIndex = vertices.Count - 1;
            baseIndex = southPoleIndex - ringVertexCount;
            for (var i = 0; i < sliceCount; i++) {
                indices.Add(southPoleIndex);
                indices.Add(baseIndex + i);
                indices.Add(baseIndex + i + 1);
            }

            return new MeshData(vertices, indices);
        }

        public static MeshData CreateCylinder(float bottomRadius, float topRadius, float height, int sliceCount, int stackCount) {
            var stackHeight = height / stackCount;
            var radiusStep = (topRadius - bottomRadius) / stackCount;
            var ringCount = stackCount + 1;

            var vertices = new List<MeshVertex>();

            for (var i = 0; i < ringCount; i++) {
                var y = -0.5f * height + i * stackHeight;
                var r = bottomRadius + i * radiusStep;
                var dTheta = 2.0f * MathF.PI / sliceCount;

                for (var j = 0; j <= sliceCount; j++) {
                    var c = MathF.Cos(j * dTheta);
                    var s = MathF.Sin(j * dTheta);

                    var v = new Vector3(r * c, y, r * s);
                    var uv = new Vector2((float)j / sliceCount, 1.0f - (float)i / stackCount);
                    var t = new Vector3(-s, 0.0f, c);

                    var dr = bottomRadius - topRadius;
                    var bitangent = new Vector3(dr * c, -height, dr * s);

                    var n = Vector3.Normalize(Vector3.Cross(t, bitangent));

                    vertices.Add(new MeshVertex(v, n, t, uv));
                }
            }

            var indices = new List<int>();

            var ringVertexCount = sliceCount + 1;
            for (var i = 0; i < stackCount; i++) {
                for (var j = 0; j < sliceCount; j++) {
                    indices.Add(i * ringVertexCount + j);
                    indices.Add((i + 1) * ringVertexCount + j);
                    indices.Add((i + 1) * ringVertexCount + j + 1);

                    indices.Add(i * ringVertexCount + j);
                    indices.Add((i + 1) * ringVertexCount + j + 1);
                    indices.Add(i * ringVertexCount + j + 1);
                }
            }

            BuildCylinderTopCap(topRadius, height, sliceCount, vertices, indices);
            BuildCylinderBottomCap(bottomRadius, height, sliceCount, vertices, indices);

            return new MeshData(vertices, indices);
        }

        public static MeshData CreateGrid(float width, float depth, int m, int n) {
            var halfWidth = width * 0.5f;
            var halfDepth = depth * 0.5f;

            var dx = width / (n - 1);
            var dz = depth / (m - 1);

            var du = 1.0f / (n - 1);
            var dv = 1.0f / (m - 1);

            var vercies = new List<MeshVertex>();

            for (var i = 0; i < m; i++) {
                var z = halfDepth - i * dz;
                for (var j = 0; j < n; j++) {
                    var x = -halfWidth + j * dx;
                    vercies.Add(new MeshVertex(new Vector3(x, 0, z), new Vector3(0, 1, 0), new Vector3(1, 0, 0), new Vector2(j * du, i * dv)));
                }
            }

            var indices = new List<int>();

            for (var i = 0; i < m - 1; i++) {
                for (var j = 0; j < n - 1; j++) {
                    indices.Add(i * n + j);
                    indices.Add(i * n + j + 1);
                    indices.Add((i + 1) * n + j);

                    indices.Add((i + 1) * n + j);
                    indices.Add(i * n + j + 1);
                    indices.Add((i + 1) * n + j + 1);
                }
            }

            return new MeshData(vercies, indices);
        }

        public static MeshData CreateFullScreenQuad() {
            var vertices = new List<MeshVertex>();
            var indices = new List<int>();

            vertices.Add(new MeshVertex(-1, -1, 0, 0, 0, -1, 1, 0, 0, 0, 1));
            vertices.Add(new MeshVertex(-1, 1, 0, 0, 0, -1, 1, 0, 0, 0, 0));
            vertices.Add(new MeshVertex(1, 1, 0, 0, 0, -1, 1, 0, 0, 1, 0));
            vertices.Add(new MeshVertex(1, -1, 0, 0, 0, -1, 1, 0, 0, 1, 1));

            indices.AddRange(new[] { 0, 1, 2, 0, 2, 3 });

            return new MeshData(vertices, indices);
        }

        private static void BuildCylinderTopCap(float topRadius, float height, int sliceCount, IList<MeshVertex> vertices, IList<int> indices) {
            var baseIndex = vertices.Count;

            var y = 0.5f * height;
            var dTheta = 2.0f * MathF.PI / sliceCount;

            for (var i = 0; i <= sliceCount; i++) {
                var x = topRadius * MathF.Cos(i * dTheta);
                var z = topRadius * MathF.Sin(i * dTheta);

                var u = x / height + 0.5f;
                var v = z / height + 0.5f;
                vertices.Add(new MeshVertex(x, y, z, 0, 1, 0, 1, 0, 0, u, v));
            }
            vertices.Add(new MeshVertex(0, y, 0, 0, 1, 0, 1, 0, 0, 0.5f, 0.5f));

            var centerIndex = vertices.Count - 1;
            for (var i = 0; i < sliceCount; i++) {
                indices.Add(centerIndex);
                indices.Add(baseIndex + i + 1);
                indices.Add(baseIndex + i);
            }
        }

        private static void BuildCylinderBottomCap(float bottomRadius, float height, int sliceCount, IList<MeshVertex> vertices, IList<int> indices) {
            var baseIndex = vertices.Count;

            var y = -0.5f * height;
            var dTheta = 2.0f * MathF.PI / sliceCount;

            for (var i = 0; i <= sliceCount; i++) {
                var x = bottomRadius * MathF.Cos(i * dTheta);
                var z = bottomRadius * MathF.Sin(i * dTheta);

                var u = x / height + 0.5f;
                var v = z / height + 0.5f;
                vertices.Add(new MeshVertex(x, y, z, 0, -1, 0, 1, 0, 0, u, v));
            }
            vertices.Add(new MeshVertex(0, y, 0, 0, -1, 0, 1, 0, 0, 0.5f, 0.5f));

            var centerIndex = vertices.Count - 1;
            for (var i = 0; i < sliceCount; i++) {
                indices.Add(centerIndex);
                indices.Add(baseIndex + i);
                indices.Add(baseIndex + i + 1);
            }
        }

        private static readonly List<Vector3> IcosahedronVertices = new List<Vector3> {
            new Vector3(-0.525731f, 0, 0.850651f), new Vector3(0.525731f, 0, 0.850651f),
            new Vector3(-0.525731f, 0, -0.850651f), new Vector3(0.525731f, 0, -0.850651f),
            new Vector3(0, 0.850651f, 0.525731f), new Vector3(0, 0.850651f, -0.525731f),
            new Vector3(0, -0.850651f, 0.525731f), new Vector3(0, -0.850651f, -0.525731f),
            new Vector3(0.850651f, 0.525731f, 0), new Vector3(-0.850651f, 0.525731f, 0),
            new Vector3(0.850651f, -0.525731f, 0), new Vector3(-0.850651f, -0.525731f, 0)
        };

        private static readonly IReadOnlyList<int> IcosahedronIndices = new List<int> {
            1,4,0,  4,9,0,  4,5,9,  8,5,4,  1,8,4,
            1,10,8, 10,3,8, 8,3,5,  3,2,5,  3,7,2,
            3,10,7, 10,6,7, 6,11,7, 6,0,11, 6,1,0,
            10,1,6, 11,0,9, 2,11,9, 5,2,9,  11,2,7
        };

    }
}
