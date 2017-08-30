using System.Drawing;

namespace OpenMLTD.MilliSim.Rendering {
    public abstract class Element2D : VisualElement, IElement2D {

        public virtual Point Location { get; set; }

    }
}
