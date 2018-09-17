using System;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Foundation.Extending;

namespace OpenMLTD.MilliSim.Extension.Components.CoreComponents {
    /// <inheritdoc />
    /// <summary>
    /// A <see cref="BaseGameComponentFactory"/> that creates <see cref="SyncTimer"/>.
    /// </summary>
    [MilliSimPlugin(typeof(IBaseGameComponentFactory))]
    public sealed class SyncTimerFactory : BaseGameComponentFactory {

        public override string PluginID => "plugin.component_factory.sync_timer";

        public override string PluginName => "SyncTimer Component Factory";

        public override string PluginDescription => "SyncTimer Component Factory";

        public override string PluginAuthor => "OpenMLTD";

        public override Version PluginVersion => MyVersion;

        public override IBaseGameComponent CreateComponent(BaseGame game, IBaseGameComponentContainer parent) {
            return new SyncTimer(game, parent);
        }

        private static readonly Version MyVersion = new Version(1, 0, 0, 0);

    }
}
