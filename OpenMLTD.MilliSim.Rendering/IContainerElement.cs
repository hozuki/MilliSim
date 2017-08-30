using System.Collections.Generic;
using JetBrains.Annotations;

namespace OpenMLTD.MilliSim.Rendering {
    public interface IContainerElement : IElement {

        [NotNull, ItemNotNull]
        IReadOnlyList<IElement> Elements { get; }

    }
}
