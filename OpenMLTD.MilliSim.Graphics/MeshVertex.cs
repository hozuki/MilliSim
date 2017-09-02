using System.Runtime.InteropServices;
using SharpDX;

namespace OpenMLTD.MilliSim.Graphics {

    [StructLayout(LayoutKind.Sequential)]
    public struct MeshVertex {

        public MeshVertex(Vector3 pos, Vector3 norm, Vector2 uv) {
            Position = pos;
            Normal = norm;
            TexCoords = uv;
        }

        public MeshVertex(float px, float py, float pz, float nx, float ny, float nz, float u, float v) :
            this(new Vector3(px, py, pz), new Vector3(nx, ny, nz), new Vector2(u, v)) {
        }

        public Vector3 Position { get; set; }

        public Vector3 Normal { get; set; }

        public Vector2 TexCoords { get; set; }

    }

}
