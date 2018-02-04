using JetBrains.Annotations;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Graphics;

namespace OpenMLTD.MilliSim.Extension.Components.ScoreComponents {
    public class MltdStage : VisualContainer {

        public MltdStage([NotNull] BaseGame game, [NotNull] IVisualContainer parent)
            : base(game, parent) {
        }

    }
}
