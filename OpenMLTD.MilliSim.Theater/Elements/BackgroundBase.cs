using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Graphics;
using OpenMLTD.MilliSim.Graphics.Drawing;

namespace OpenMLTD.MilliSim.Theater.Elements {
    public abstract class BackgroundBase : Element2D {

        protected BackgroundBase(GameBase game)
            : base(game) {
        }

        public override string Name { get; set; } = "Background";

    }
}
