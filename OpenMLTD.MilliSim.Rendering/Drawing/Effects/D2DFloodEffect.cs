using System.Drawing;
using OpenMLTD.MilliSim.Rendering.Extensions;
using SharpDX.Direct2D1;

namespace OpenMLTD.MilliSim.Rendering.Drawing.Effects {
    public sealed class D2DFloodEffect : D2DEffect {

        public D2DFloodEffect(Effect effect)
            : base(effect) {
        }

        public D2DFloodEffect(DeviceContext deviceContext)
            : base(new Effect(deviceContext, Effect.Flood)) {
        }

        public Color Color {
            get {
                var rawColor = NativeEffect.GetColor4Value((int)FloodProperties.Color);
                return rawColor.ToColor();
            }
            set {
                var rawColor = value.ToRC4();
                NativeEffect.SetValue((int)FloodProperties.Color, rawColor);
            }
        }

    }
}
