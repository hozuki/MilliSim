using System.Drawing;
using OpenMLTD.MilliSim.Graphics.Extensions;
using SharpDX.Direct2D1;

namespace OpenMLTD.MilliSim.Graphics.Drawing.Direct2D.Effects {
    public sealed class D2DFloodEffect : D2DEffect {

        public D2DFloodEffect(Effect effect)
            : base(effect) {
        }

        public D2DFloodEffect(RenderContext context)
            : this(context.RenderTarget.DeviceContext) {
        }

        public D2DFloodEffect(DeviceContext context)
            : base(new Effect(context, Effect.Flood)) {
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
