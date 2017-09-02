using System.Linq;
using OpenMLTD.MilliSim.Graphics.Extensions;
using OpenMLTD.MilliSim.Graphics.Rendering.Direct3D;
using SharpDX;
using SharpDX.Direct3D11;
using Color = System.Drawing.Color;

namespace OpenMLTD.MilliSim.Graphics.Rendering {
    public class BasicModel : ModelBase<MeshVertex> {

        private BasicModel() {
        }

        public static BasicModel CreateBox(Device device, float width, float height, float depth) {
            var model = new BasicModel();
            model.CreateBoxInternal(device, width, height, depth);
            return model;
        }

        public static BasicModel CreateSphere(Device device, float radius, int slices, int stacks) {
            var model = new BasicModel();
            model.CreateSphereInternal(device, radius, slices, stacks);
            return model;
        }

        public static BasicModel CreateCylinder(Device device, float bottomRadius, float topRadius, float height, int sliceCount, int stackCount) {
            var model = new BasicModel();
            model.CreateCylinderInternal(device, bottomRadius, topRadius, height, sliceCount, stackCount);
            return model;
        }

        public static BasicModel CreateGrid(Device device, float width, float depth, int xVerts, int yVerts) {
            var model = new BasicModel();
            model.CreateGridInternal(device, width, depth, xVerts, yVerts);
            return model;
        }

        private void CreateBoxInternal(Device device, float width, float height, float depth) {
            var box = GeometryGenerator.CreateBox(width, height, depth);
            InitFromMeshData(device, box);
        }

        private void CreateSphereInternal(Device device, float radius, int slices, int stacks) {
            var sphere = GeometryGenerator.CreateSphere(radius, slices, stacks);
            InitFromMeshData(device, sphere);
        }

        private void CreateCylinderInternal(Device device, float bottomRadius, float topRadius, float height, int sliceCount, int stackCount) {
            var cylinder = GeometryGenerator.CreateCylinder(bottomRadius, topRadius, height, sliceCount, stackCount);
            InitFromMeshData(device, cylinder);
        }

        private void CreateGridInternal(Device device, float width, float depth, int xVerts, int zVerts) {
            var grid = GeometryGenerator.CreateGrid(width, depth, xVerts, zVerts);
            InitFromMeshData(device, grid);
        }

        private void InitFromMeshData(Device device, MeshData mesh) {
            var max = new Vector3(float.MinValue);
            var min = new Vector3(float.MaxValue);

            foreach (var vertex in mesh.Vertices) {
                max = MathF.Maximize(max, vertex.Position);
                min = MathF.Minimize(min, vertex.Position);
            }

            BoundingBox = new BoundingBox(min, max);

            // Redundant copy
            var vertices = mesh.Vertices.ToArray();
            var indices = mesh.Indices.ToArray();
            _vertices = vertices;
            _indices = indices;

            Mesh.SetVertices(device, vertices);
            Mesh.SetIndices(device, indices);

            Material = DefaultMaterial;
        }

        private static readonly D3DMaterial DefaultMaterial = new D3DMaterial {
            Ambient = Color.White.ToRC4(),
            Diffuse = Color.White.ToRC4(),
            Reflect = Color.White.ToRC4(),
            Specular = new Color4(Color3.White, 16f)
        };

    }
}
