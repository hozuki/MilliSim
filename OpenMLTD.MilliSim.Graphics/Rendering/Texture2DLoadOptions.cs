using SharpDX.Direct3D11;
using SharpDX.DXGI;

namespace OpenMLTD.MilliSim.Graphics.Rendering {

    public struct Texture2DLoadOptions {

        public Format Format;
        public ResourceUsage ResourceUsage;
        public BindFlags BindFlags;
        public CpuAccessFlags CpuAccessFlags;
        public int MipLevels;

    }

}
