using JetBrains.Annotations;
using OpenMLTD.MilliSim.Configuration;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Graphics.Drawing;

namespace OpenMLTD.MilliSim.Graphics {
    public sealed class Stage : BufferedVisualContainer {

        internal Stage([NotNull] VisualGame game, [NotNull] ConfigurationStore store)
            : base(null) {
            Game = game;
            ConfigurationStore = store;
        }

        internal void Draw([NotNull] GameTime gameTime, [NotNull] ControlStageRenderer renderer) {
            renderer.Draw((IVisualContainer)Game.Root, gameTime);
        }

    }
}
