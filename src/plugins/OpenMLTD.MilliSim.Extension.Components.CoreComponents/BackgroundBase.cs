using JetBrains.Annotations;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Graphics;

namespace OpenMLTD.MilliSim.Extension.Components.CoreComponents {
    public abstract class BackgroundBase : Visual {

        protected BackgroundBase([NotNull] BaseGame game, [NotNull] IVisualContainer parent)
            : base(game, parent) {
        }

    }
}
