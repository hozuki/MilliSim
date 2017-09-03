using JetBrains.Annotations;

namespace OpenMLTD.MilliSim.Foundation {
    public interface IContainerElement : IElement {

        [NotNull, ItemNotNull]
        ElementCollection Elements { get; }

    }
}
