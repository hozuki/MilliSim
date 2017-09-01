using System.Collections.Generic;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Graphics;
using OpenMLTD.MilliSim.Graphics.Drawing;

namespace OpenMLTD.MilliSim.Theater.Elements {
    public class Stage : ContainerElement {

        public Stage(GameBase game, [CanBeNull] [ItemNotNull] IReadOnlyList<IElement> elements)
            : base(game, elements) {
        }

    }
}
