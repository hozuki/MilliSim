using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;

namespace OpenMLTD.MilliSim.Extension.Components.ScoreComponents.Internal {
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Material {

        public Vector4 Ambient;
        public Vector4 Diffuse;
        public Vector4 Specular;
        public Vector4 Reflect;

    }
}
