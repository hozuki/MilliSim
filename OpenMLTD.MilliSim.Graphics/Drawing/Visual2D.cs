using System.Drawing;
using JetBrains.Annotations;

namespace OpenMLTD.MilliSim.Graphics.Drawing {
    public abstract class Visual2D : Visual, IVisual2D {

        protected Visual2D([NotNull] IVisualContainer parent)
            : base(parent) {
        }

        public virtual Point Location { get; set; }

    }
}
