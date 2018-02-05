using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpenMLTD.MilliSim.Extension.Components.ScoreComponents.Effects;
using OpenMLTD.MilliSim.Extension.Components.ScoreComponents.Internal;

namespace OpenMLTD.MilliSim.Extension.Components.ScoreComponents.Extensions {
    internal static class GraphicsDeviceExtensions {

        internal static void DrawRibbon([NotNull] this GraphicsDevice graphicsDevice, RibbonMesh mesh, [NotNull] RibbonEffect effect, Matrix viewProjection, Material material, [NotNull] Texture2D ribbonTexture) {
            // World = I, so use viewProjection as World*viewProjection
            effect.World = Matrix.Identity;
            effect.WorldInvTranspose = Matrix.Identity;
            effect.WorldViewProj = viewProjection;

            effect.MaterialAmbient = material.Ambient;
            effect.MaterialDiffuse = material.Diffuse;
            effect.MaterialSpecular = material.Specular;
            effect.MaterialReflect = material.Reflect;

            effect.Opacity = 1;

            effect.TextureTransform = Matrix.Identity;
            effect.RibbonTexture = ribbonTexture;

            var pass = effect.CurrentTechnique.Passes[0];

            pass.Apply();

            graphicsDevice.BlendState = BlendState.AlphaBlend;
            graphicsDevice.DepthStencilState = DepthStencilState.Default;
            graphicsDevice.RasterizerState = RasterizerState.CullNone;

            mesh.Draw(graphicsDevice);
        }

    }
}

