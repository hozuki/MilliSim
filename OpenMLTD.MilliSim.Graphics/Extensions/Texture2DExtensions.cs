using SharpDX.Direct3D11;

namespace OpenMLTD.MilliSim.Graphics.Extensions {
    public static class Texture2DExtensions {

        public static ShaderResourceView CreateResourceView(this Texture2D texture2D) {
            return new ShaderResourceView(texture2D.Device, texture2D);
        }

    }
}
