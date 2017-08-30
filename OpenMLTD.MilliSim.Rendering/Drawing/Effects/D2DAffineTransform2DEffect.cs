using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;

namespace OpenMLTD.MilliSim.Rendering.Drawing.Effects {
    public sealed class D2DAffineTransform2DEffect : D2DEffect {

        public D2DAffineTransform2DEffect(Effect effect)
            : base(effect) {
        }

        public D2DAffineTransform2DEffect(RenderContext context)
            : this(context.RenderTarget.DeviceContext) {
        }

        public D2DAffineTransform2DEffect(DeviceContext context)
            : base(new Effect(context, Effect.AffineTransform2D)) {
        }

        public AffineTransform2DInterpolationMode InterpolationMode {
            get => NativeEffect.GetEnumValue<AffineTransform2DInterpolationMode>((int)AffineTransform2DProperties.InterpolationMode);
            set => NativeEffect.SetEnumValue((int)AffineTransform2DProperties.InterpolationMode, value);
        }

        public BorderMode BorderMode {
            get => NativeEffect.GetEnumValue<BorderMode>((int)AffineTransform2DProperties.BorderMode);
            set => NativeEffect.SetEnumValue((int)AffineTransform2DProperties.BorderMode, value);
        }

        public float Sharpness {
            get => NativeEffect.GetFloatValue((int)AffineTransform2DProperties.Sharpness);
            set => NativeEffect.SetValue((int)AffineTransform2DProperties.Sharpness, value);
        }

        // Warning: directly setting this value will lose track of transformation components.
        public Matrix3x2 TransformMatrix {
            get => NativeEffect.GetMatrix3x2Value((int)AffineTransform2DProperties.TransformMatrix);
            set => NativeEffect.SetValue((int)AffineTransform2DProperties.TransformMatrix, (RawMatrix3x2)value);
        }

        public Vector2 Translation {
            get => _translation;
            set {
                _translation = value;
                UpdateMatrix();
            }
        }

        public Vector2 Scaling {
            get => _scaling;
            set {
                _scaling = value;
                UpdateMatrix();
            }
        }

        public float Rotation {
            get => _rotation;
            set {
                _rotation = value;
                UpdateMatrix();
            }
        }

        private void UpdateMatrix() {
            var matrix = Matrix3x2.Transformation(Scaling.X, Scaling.Y, Rotation, Translation.X, Translation.Y);
            TransformMatrix = matrix;
        }

        private Vector2 _translation;
        private Vector2 _scaling;
        private float _rotation;

    }
}
