using JetBrains.Annotations;
using OpenMLTD.MilliSim.Configuration;
using OpenMLTD.MilliSim.Foundation;

namespace OpenMLTD.MilliSim.Graphics {
    public sealed class Stage : VisualContainer {

        public Stage([NotNull] BaseGame game, [NotNull] ConfigurationStore configurationStore)
            // ReSharper disable once AssignNullToNotNullAttribute
            : base(game, null) {
            ConfigurationStore = configurationStore;
        }

    }
}
