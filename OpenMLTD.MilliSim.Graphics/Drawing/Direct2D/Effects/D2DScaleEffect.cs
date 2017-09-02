using System.Drawing;
using OpenMLTD.MilliSim.Graphics.Extensions;
using SharpDX.Direct2D1;

namespace OpenMLTD.MilliSim.Graphics.Drawing.Direct2D.Effects {
    public sealed class D2DScaleEffect : D2DEffect {

        public D2DScaleEffect(Effect effect)
            : base(effect) {
        }

        public D2DScaleEffect(RenderContext context)
            : this(context.RenderTarget.DeviceContext2D) {
        }

        public D2DScaleEffect(DeviceContext context)
            : base(new Effect(context, Effect.Scale)) {
        }

        public ScaleInterpolationMode InterpolationMode {
            get => NativeEffect.GetEnumValue<ScaleInterpolationMode>((int)ScaleProperties.InterpolationMode);
            set => NativeEffect.SetEnumValue((int)ScaleProperties.InterpolationMode, value);
        }

        public BorderMode BorderMode {
            get => NativeEffect.GetEnumValue<BorderMode>((int)ScaleProperties.BorderMode);
            set => NativeEffect.SetEnumValue((int)ScaleProperties.BorderMode, value);
        }

        public float Sharpness {
            get => NativeEffect.GetFloatValue((int)ScaleProperties.Sharpness);
            set => NativeEffect.SetValue((int)ScaleProperties.Sharpness, value);
        }

        public SizeF Scale {
            get {
                var vec = NativeEffect.GetVector2Value((int)ScaleProperties.Scale);
                return vec.ToGdiSizeF();
            }
            set {
                var vec = value.ToD2DVector();
                NativeEffect.SetValue((int)ScaleProperties.Scale, vec);
            }
        }

        public float ScaleX {
            get => Scale.Width;
            set => Scale = new SizeF(value, Scale.Height);
        }

        public float ScaleY {
            get => Scale.Height;
            set => Scale = new SizeF(Scale.Width, value);
        }

        public PointF CenterPoint {
            get {
                var pt = NativeEffect.GetVector2Value((int)ScaleProperties.CenterPoint);
                return pt.ToGdiPointF();
            }
            set {
                var vec = value.ToD2DVector();
                NativeEffect.SetValue((int)ScaleProperties.CenterPoint, vec);
            }
        }

    }
}
