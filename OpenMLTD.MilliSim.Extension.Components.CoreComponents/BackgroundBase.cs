using JetBrains.Annotations;
using OpenMLTD.MilliSim.Graphics;
using OpenMLTD.MilliSim.Graphics.Drawing;

namespace OpenMLTD.MilliSim.Extension.Components.CoreComponents {
    public abstract class BackgroundBase : Visual2D {

        protected BackgroundBase([NotNull] IVisualContainer parent)
            : base(parent) {
        }

    }
}
