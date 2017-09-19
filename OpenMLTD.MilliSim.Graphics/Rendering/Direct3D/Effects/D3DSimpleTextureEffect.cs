using JetBrains.Annotations;
using SharpDX;
using SharpDX.Direct3D11;

namespace OpenMLTD.MilliSim.Graphics.Rendering.Direct3D.Effects {
    public sealed class D3DSimpleTextureEffect : D3DEffect {

        private D3DSimpleTextureEffect(RenderContext context, string textSource, bool sourceIsText)
            : this(context.Direct3DDevice, textSource, sourceIsText) {
        }

        private D3DSimpleTextureEffect(Device device, string textSource, bool sourceIsText)
            : base(device, textSource, sourceIsText) {
        }

        public EffectTechnique SimpleTextureTechnique => _simpleTextureTech;

        public Matrix WorldTransform {
            get => _world.GetMatrix();
            set => _world.SetMatrix(value);
        }

        public Matrix WorldTransformInversedTransposed {
            get => _worldInvTranspose.GetMatrix();
            set => _worldInvTranspose.SetMatrix(value);
        }

        public Matrix WorldViewProjectionTransform {
            get => _worldViewProj.GetMatrix();
            set => _worldViewProj.SetMatrix(value);
        }

        public Matrix TextureTransform {
            get => _texTransform.GetMatrix();
            set => _texTransform.SetMatrix(value);
        }

        public D3DMaterial Material {
            get => ReadStruct<D3DMaterial>(_material);
            set => WriteStruct(_material, value);
        }

        public float CurrentTime {
            get => ReadStruct<float>(_currentTime);
            set => WriteStruct(_currentTime, value);
        }

        [CanBeNull]
        public ShaderResourceView Texture {
            get => _diffuseMap.GetResource();
            set => _diffuseMap.SetResource(value);
        }

        internal override void Initialize() {
            var effect = NativeEffect;

            _simpleTextureTech = effect.GetTechniqueByName("SimpleTexture");

            // Variables

            _world = effect.GetVariableByName("gWorld").AsMatrix();
            _worldInvTranspose = effect.GetVariableByName("gWorldInvTranspose").AsMatrix();
            _worldViewProj = effect.GetVariableByName("gWorldViewProj").AsMatrix();
            _texTransform = effect.GetVariableByName("gTexTransform").AsMatrix();
            _currentTime = effect.GetVariableByName("gCurrentTime");
            _material = effect.GetVariableByName("gMaterial");

            _diffuseMap = effect.GetVariableByName("gDiffuseMap").AsShaderResource();
        }

        private EffectTechnique _simpleTextureTech;

        private EffectMatrixVariable _world;
        private EffectMatrixVariable _worldInvTranspose;
        private EffectMatrixVariable _worldViewProj;
        private EffectMatrixVariable _texTransform;
        private EffectVariable _currentTime;
        private EffectVariable _material;

        private EffectShaderResourceVariable _diffuseMap;

    }
}
