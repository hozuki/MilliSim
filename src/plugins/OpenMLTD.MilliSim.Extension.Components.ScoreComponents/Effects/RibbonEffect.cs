using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OpenMLTD.MilliSim.Extension.Components.ScoreComponents.Effects {
    public sealed class RibbonEffect : Effect {

        public RibbonEffect(GraphicsDevice graphicsDevice, byte[] effectCode)
            : base(graphicsDevice, effectCode) {
            Initialize();
        }

        public Vector4 MaterialAmbient {
            get => _materialAmbient?.GetValueVector4() ?? Vector4.Zero;
            set => _materialAmbient?.SetValue(value);
        }

        public Vector4 MaterialDiffuse {
            get => _materialDiffuse?.GetValueVector4() ?? Vector4.Zero;
            set => _materialDiffuse?.SetValue(value);
        }

        public Vector4 MaterialSpecular {
            get => _materialSpecular?.GetValueVector4() ?? Vector4.Zero;
            set => _materialDiffuse?.SetValue(value);
        }

        public Vector4 MaterialReflect {
            get => _materialReflect?.GetValueVector4() ?? Vector4.Zero;
            set => _materialReflect?.SetValue(value);
        }

        public Matrix World {
            get => _world.GetValueMatrix();
            set => _world.SetValue(value);
        }

        public Matrix WorldInvTranspose {
            get => _worldInvTranspose.GetValueMatrix();
            set => _worldInvTranspose.SetValue(value);
        }

        public Matrix WorldViewProj {
            get => _worldViewProj.GetValueMatrix();
            set => _worldViewProj.SetValue(value);
        }

        public Matrix TextureTransform {
            get => _textureTransform.GetValueMatrix();
            set => _textureTransform.SetValue(value);
        }

        public Texture2D RibbonTexture {
            get => _ribbonTexture?.GetValueTexture2D();
            set => _ribbonTexture?.SetValue(value);
        }

        public float CurrentTime {
            get => _currentTime?.GetValueSingle() ?? 0;
            set => _currentTime?.SetValue(value);
        }

        public float Opacity {
            get => _opacity.GetValueSingle();
            set => _opacity.SetValue(MathHelper.Clamp(value, 0, 1));
        }

        private void Initialize() {
            var p = Parameters;

            _materialAmbient = p["gMaterialAmbient"];
            _materialDiffuse = p["gMaterialDiffuse"];
            _materialSpecular = p["gMaterialSpecular"];
            _materialReflect = p["gMaterialReflect"];
            _world = p["gWorld"];
            _worldInvTranspose = p["gWorldInvTranspose"];
            _worldViewProj = p["gWorldViewProj"];
            _textureTransform = p["gTextureTransform"];
            _ribbonTexture = p["gRibbonTexture"];
            _currentTime = p["gCurrentTime"];
            _opacity = p["gOpacity"];
        }

        [CanBeNull]
        private EffectParameter _materialAmbient;
        [CanBeNull]
        private EffectParameter _materialDiffuse;
        [CanBeNull]
        private EffectParameter _materialSpecular;
        [CanBeNull]
        private EffectParameter _materialReflect;
        private EffectParameter _world;
        private EffectParameter _worldInvTranspose;
        private EffectParameter _worldViewProj;
        private EffectParameter _textureTransform;
        private EffectParameter _ribbonTexture;
        [CanBeNull]
        private EffectParameter _currentTime;
        private EffectParameter _opacity;

    }
}
