using System.Drawing;
using OpenMLTD.MilliSim.Foundation;

namespace OpenMLTD.MilliSim.Graphics.Drawing {
    public interface IElement2D : IElement {

        Point Location { get; set; }

    }
}
