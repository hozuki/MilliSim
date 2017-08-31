using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Rendering;

namespace OpenMLTD.MilliSim.Theater.Elements {
    public abstract class BackgroundBase : Element2D {

        protected BackgroundBase(GameBase game)
            : base(game) {
        }

        public override string Name { get; set; } = "Background";

    }
}
