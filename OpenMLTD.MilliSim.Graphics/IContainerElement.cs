using System.Collections.Generic;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Foundation;

namespace OpenMLTD.MilliSim.Graphics {
    public interface IContainerElement : IElement {

        [NotNull, ItemNotNull]
        IReadOnlyList<IElement> Elements { get; }

    }
}
