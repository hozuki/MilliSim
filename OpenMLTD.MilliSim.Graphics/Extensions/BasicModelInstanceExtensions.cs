using OpenMLTD.MilliSim.Graphics.Rendering;
using OpenMLTD.MilliSim.Graphics.Rendering.Direct3D.Effects;
using SharpDX;
using SharpDX.Direct3D11;

namespace OpenMLTD.MilliSim.Graphics.Extensions {
    public static class BasicModelInstanceExtensions {

        public static void Draw(this BasicModelInstance instance, DeviceContext deviceContext, D3DSimpleTextureEffect effect, Matrix viewProjection) {
            var world = instance.World;
            var wit = MathF.InverseTranspose(world);
            var wvp = world * viewProjection;

            effect.WorldTransform = world;
            effect.WorldTransformInversedTransposed = wit;
            effect.WorldViewProjectionTransform = wvp;

            effect.Material = instance.Model.Material;
            effect.Texture = instance.Model.DiffuseMap;

            DrawTech(instance, deviceContext, effect.SimpleTextureTechnique);
        }

        public static void Draw(this BasicModelInstance instance, DeviceContext deviceContext, D3DVertexColorEffect effect, Matrix viewProjection) {
            var wvp = instance.World * viewProjection;

            effect.WorldViewProjection = wvp;
            effect.Material = instance.Model.Material;

            DrawTech(instance, deviceContext, effect.VertexColorTechnique);
        }

        private static void DrawTech(BasicModelInstance instance, DeviceContext deviceContext, EffectTechnique technique) {
            var desc = technique.Description;
            for (var i = 0; i < desc.PassCount; ++i) {
                technique.GetPassByIndex(i).Apply(deviceContext);
                instance.Model.Mesh.Draw(deviceContext);
            }
        }

    }
}
