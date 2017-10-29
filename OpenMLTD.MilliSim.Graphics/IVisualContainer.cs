using JetBrains.Annotations;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Foundation;

namespace OpenMLTD.MilliSim.Graphics {
    public interface IVisualContainer : IComponentContainer, IDrawable {

        void OnBeforeChildrenDraw([NotNull] GameTime gameTime, [NotNull] RenderContext context);

        void OnAfterChildrenDraw([NotNull] GameTime gameTime, [NotNull] RenderContext context);

    }
}
