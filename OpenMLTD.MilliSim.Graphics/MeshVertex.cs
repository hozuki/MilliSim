using System.Runtime.InteropServices;
using SharpDX;

namespace OpenMLTD.MilliSim.Graphics {

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct MeshVertex {

        public MeshVertex(Vector3 position, Vector3 normal, Vector3 tangent, Vector2 texCoords) {
            _position = position;
            _normal = normal;
            _texCoords = texCoords;
            _tangent = tangent;
        }

        public MeshVertex(float px, float py, float pz,
            float nx, float ny, float nz,
            float tx, float ty, float tz,
            float u, float v) :
            this(new Vector3(px, py, pz), new Vector3(nx, ny, nz), new Vector3(tx, ty, tz), new Vector2(u, v)) {
        }

        public Vector3 Position {
            get => _position;
            set => _position = value;
        }

        public Vector3 Normal {
            get => _normal;
            set => _normal = value;
        }

        public Vector3 Tangent {
            get => _tangent;
            set => _tangent = value;
        }

        public Vector2 TexCoords {
            get => _texCoords;
            set => _texCoords = value;
        }

        // Please note that the layout of these fields must be the same as declared in InputLayoutDescriptions.PosNormTexTan.
        // Also it must be the same as PS_IN structure declared in helper.fx.
        // That's why I explicitly created these fields rather than using auto properties.
        // Traditionally, the order of parameters are "pos, norm, tan, tex" (3+3+3+2), but the order of a optimized PS_IN structrue
        // should be "pos, norm, tex, tan" (3+3+2+3), as the latter aligns better.

        private Vector3 _position;

        private Vector3 _normal;

        private Vector2 _texCoords;

        private Vector3 _tangent;

    }

}
