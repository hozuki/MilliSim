using System.Drawing;
using OpenMLTD.MilliSim.Foundation;

namespace OpenMLTD.MilliSim.Rendering {
    public abstract class Element2D : VisualElement, IElement2D {

        protected Element2D(GameBase game)
            : base(game) {
        }

        public virtual Point Location { get; set; }

    }
}
