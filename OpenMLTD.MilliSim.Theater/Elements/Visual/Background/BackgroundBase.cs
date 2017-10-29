using JetBrains.Annotations;
using OpenMLTD.MilliSim.Graphics;
using OpenMLTD.MilliSim.Graphics.Drawing;

namespace OpenMLTD.MilliSim.Theater.Elements.Visual.Background {
    public abstract class BackgroundBase : Visual2D {

        protected BackgroundBase([NotNull] IVisualContainer parent)
            : base(parent) {
        }

    }
}
