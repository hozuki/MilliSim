using System.Collections.Generic;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Graphics.Drawing;

namespace OpenMLTD.MilliSim.Theater.Elements {
    public class GamingArea : ContainerElement {

        public GamingArea(GameBase game, [CanBeNull] [ItemNotNull] IReadOnlyList<IElement> elements)
            : base(game, elements) {
        }

    }
}
