using System.Collections.Generic;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Graphics.Drawing;

namespace OpenMLTD.MilliSim.Graphics {
    public sealed class Stage : BufferedContainerElement {

        public Stage(VisualGame game, IReadOnlyList<IElement> elements)
            : base(game, elements) {
        }

        internal void Draw([NotNull] GameTime gameTime, [NotNull] ControlStageRenderer renderer) {
            renderer.Draw((IVisualContainerElement)Game.Root, gameTime);
        }

    }
}
