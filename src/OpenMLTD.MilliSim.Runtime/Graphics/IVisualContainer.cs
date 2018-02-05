using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using OpenMLTD.MilliSim.Foundation;

namespace OpenMLTD.MilliSim.Graphics {
    public interface IVisualContainer : IVisual, IBaseGameComponentContainer {

        void OnBeforeChildrenDraw([NotNull] GameTime gameTime);

        void OnAfterChildrenDraw([NotNull] GameTime gameTime);

    }
}
