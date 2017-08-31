using System.Drawing;
using OpenMLTD.MilliSim.Core;

namespace OpenMLTD.MilliSim.Foundation {
    public abstract class RendererBase : DisposableBase {

        protected RendererBase(GameBase game) {
            Game = game;
        }

        public GameBase Game { get; }

        public Color ClearColor { get; set; } = Color.Black;

        public abstract Size ClientSize { get; }

        protected internal abstract void Initialize();

    }
}
