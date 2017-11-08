using OpenMLTD.MilliSim.Extension.Components.ScoreComponents.Internal;
using OpenMLTD.MilliSim.Graphics;
using OpenMLTD.MilliSim.Graphics.Rendering.Direct3D;
using OpenMLTD.MilliSim.Graphics.Rendering.Direct3D.Effects;
using SharpDX;
using SharpDX.Direct3D11;

namespace OpenMLTD.MilliSim.Extension.Components.ScoreComponents.Extensions {
    internal static class RenderContextExtensions {

        internal static void DrawRibbon(this RenderContext context, RibbonMesh mesh, D3DSimpleTextureEffect effect, D3DMaterial material, Matrix viewProjection, ShaderResourceView texture) {
            // World = I, so use viewProjection as World*viewProjection
            effect.WorldTransform = Matrix.Identity;
            effect.WorldTransformInversedTransposed = MathF.InverseTranspose(Matrix.Identity);
            effect.WorldViewProjectionTransform = viewProjection;

            effect.Material = material;
            effect.Texture = texture;

            var tech = effect.SimpleTextureTechnique;
            var desc = tech.Description;
            var deviceContext = context.Direct3DDevice.ImmediateContext;
            for (var i = 0; i < desc.PassCount; ++i) {
                tech.GetPassByIndex(i).Apply(deviceContext);
                mesh.Draw(deviceContext);
            }
        }

    }
}

