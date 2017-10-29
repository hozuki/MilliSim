using JetBrains.Annotations;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Graphics.Drawing;

namespace OpenMLTD.MilliSim.Graphics {
    public sealed class Stage : BufferedVisualContainer {

        public Stage([NotNull] VisualGame game)
            : base(null) {
            Game = game;
        }

        internal void Draw([NotNull] GameTime gameTime, [NotNull] ControlStageRenderer renderer) {
            renderer.Draw((IVisualContainer)Game.Root, gameTime);
        }

    }
}
