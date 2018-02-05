using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OpenMLTD.MilliSim.Extension.Components.CoreComponents.Effects {
    public sealed class SpriteOutlineEffect : Effect {

        public SpriteOutlineEffect([NotNull] GraphicsDevice graphicsDevice, [NotNull] byte[] effectCode)
            : base(graphicsDevice, effectCode) {
            Initialize();
        }

        public Matrix WorldViewProjection {
            get => _worldViewProj.GetValueMatrix();
            set => _worldViewProj.SetValue(value);
        }

        public Vector4 OutlineColor {
            get => _outlineColor.GetValueVector4();
            set => _outlineColor.SetValue(value);
        }

        public float OutlineThickness {
            get => _outlineThickness.GetValueSingle();
            set {
                if (value < float.Epsilon) {
                    value = 0;
                }

                _outlineThickness.SetValue(value);
            }
        }

        public Texture2D Texture {
            get => _texture.GetValueTexture2D();
            set => _texture.SetValue(value);
        }

        public float Opacity {
            get => _opacity.GetValueSingle();
            set => _opacity.SetValue(MathHelper.Clamp(value, 0, 1));
        }

        private void Initialize() {
            var p = Parameters;

            _worldViewProj = p["gWorldViewProj"];
            _outlineColor = p["gOutlineColor"];
            _outlineThickness = p["gOutlineThickness"];
            _texture = p["gTexture"];
            _opacity = p["gOpacity"];
        }

        private EffectParameter _worldViewProj;
        private EffectParameter _outlineColor;
        private EffectParameter _outlineThickness;
        private EffectParameter _texture;
        private EffectParameter _opacity;

    }
}
