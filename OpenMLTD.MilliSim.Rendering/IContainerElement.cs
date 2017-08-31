using System.Collections.Generic;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Foundation;

namespace OpenMLTD.MilliSim.Rendering {
    public interface IContainerElement : IElement {

        [NotNull, ItemNotNull]
        IReadOnlyList<IElement> Elements { get; }

    }
}
