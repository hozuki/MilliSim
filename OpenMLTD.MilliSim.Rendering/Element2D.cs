using System.Drawing;

namespace OpenMLTD.MilliSim.Rendering {
    public abstract class Element2D : VisualElement, I2DElement {

        public virtual Point Location { get; set; }

    }
}
