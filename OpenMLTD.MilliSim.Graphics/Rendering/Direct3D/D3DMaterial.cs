using System.Runtime.InteropServices;
using SharpDX;

namespace OpenMLTD.MilliSim.Graphics.Rendering.Direct3D {
    [StructLayout(LayoutKind.Sequential)]
    public struct D3DMaterial {

        public Color4 Ambient;
        public Color4 Diffuse;
        public Color4 Specular;
        public Color4 Reflect;

        public static readonly int Stride = Marshal.SizeOf(typeof(D3DMaterial));

    }
}
