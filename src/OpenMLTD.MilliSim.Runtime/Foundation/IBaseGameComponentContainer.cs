using JetBrains.Annotations;
using Microsoft.Xna.Framework;

namespace OpenMLTD.MilliSim.Foundation {
    public interface IBaseGameComponentContainer : IBaseGameComponent {

        [NotNull, ItemNotNull]
        BaseGameComponentCollection Components { get; }

        void OnBeforeChildrenUpdate([NotNull] GameTime gameTime);

        void OnAfterChildrenUpdate([NotNull] GameTime gameTime);

    }
}
