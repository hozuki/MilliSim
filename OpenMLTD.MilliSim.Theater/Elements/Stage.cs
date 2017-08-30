using System.Collections.Generic;
using System.Drawing;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Rendering;

namespace OpenMLTD.MilliSim.Theater.Elements {
    public class Stage : ContainerElement, I2DElement {

        public Stage([CanBeNull] [ItemNotNull] IReadOnlyList<Element> elements)
            : base(elements) {
        }

        public virtual Point Location { get; set; }

    }
}
