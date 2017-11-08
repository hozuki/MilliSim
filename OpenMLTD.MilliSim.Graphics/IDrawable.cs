using System.Drawing;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Core;

namespace OpenMLTD.MilliSim.Graphics {
    public interface IDrawable {

        void Draw([NotNull] GameTime gameTime, [NotNull] RenderContext context);

        bool Visible { get; set; }

        void OnGotContext([NotNull] RenderContext context);

        void OnLostContext([NotNull] RenderContext context);

        void OnStageReady([NotNull] RenderContext context);

        void OnLayout(Size clientSize);

    }
}
