using SharpDX;
using SharpDX.Direct3D11;

namespace OpenMLTD.MilliSim.Graphics.Rendering.Direct3D.Effects {
    public sealed class D3DVertexColorEffect : D3DEffect {

        private D3DVertexColorEffect(RenderContext context, string textSource, bool sourceIsText)
            : this(context.Direct3DDevice, textSource, sourceIsText) {
        }

        private D3DVertexColorEffect(Device device, string textSource, bool sourceIsText)
            : base(device, textSource, sourceIsText) {
        }

        public EffectTechnique VertexColorTechnique => _vertexColorTech;

        public Matrix WorldViewProjection {
            get => _worldViewProj.GetMatrix();
            set => _worldViewProj.SetMatrix(value);
        }

        public D3DMaterial Material {
            get => ReadStruct<D3DMaterial>(_material);
            set => WriteStruct(_material, value);
        }

        internal override void Initialize() {
            var effect = NativeEffect;

            _vertexColorTech = effect.GetTechniqueByName("VertexColor");

            _worldViewProj = effect.GetVariableByName("gWorldViewProj").AsMatrix();
            _material = effect.GetVariableByName("gMaterial");
        }

        private EffectMatrixVariable _worldViewProj;
        private EffectVariable _material;

        private EffectTechnique _vertexColorTech;

    }
}
