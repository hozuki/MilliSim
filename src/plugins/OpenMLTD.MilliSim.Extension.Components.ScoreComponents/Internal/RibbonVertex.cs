using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;

namespace OpenMLTD.MilliSim.Extension.Components.ScoreComponents.Internal {
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct RibbonVertex {

        public RibbonVertex(Vector3 position, Vector3 normal, Vector2 texCoords) {
            Position = position;
            Normal = normal;
            TexCoords = texCoords;
        }

        public RibbonVertex(float px, float py, float pz,
            float nx, float ny, float nz,
            float u, float v) :
            this(new Vector3(px, py, pz), new Vector3(nx, ny, nz), new Vector2(u, v)) {
        }

        public Vector3 Position;

        public Vector3 Normal;

        public Vector2 TexCoords;

    }
}
