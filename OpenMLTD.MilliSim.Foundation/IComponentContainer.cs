using JetBrains.Annotations;
using OpenMLTD.MilliSim.Core;

namespace OpenMLTD.MilliSim.Foundation {
    public interface IComponentContainer : IComponent {

        [NotNull, ItemNotNull]
        ComponentCollection Components { get; }

        void OnBeforeChildrenUpdate([NotNull] GameTime gameTime);

        void OnAfterChildrenUpdate([NotNull] GameTime gameTime);

    }
}
