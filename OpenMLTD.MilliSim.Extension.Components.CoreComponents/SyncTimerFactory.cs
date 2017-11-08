using System;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Foundation;

namespace OpenMLTD.MilliSim.Extension.Components.CoreComponents {
    [MilliSimPlugin(typeof(IComponentFactory))]
    public sealed class SyncTimerFactory : ComponentFactory {

        public override string PluginID => "plugin.component_factory.sync_timer";

        public override string PluginName => "SyncTimer Component Factory";

        public override string PluginDescription => "SyncTimer Component Factory";

        public override string PluginAuthor => "OpenMLTD";

        public override Version PluginVersion => MyVersion;

        public override IComponent CreateComponent(GameBase game, IComponentContainer parent) {
            return new SyncTimer(parent);
        }

        private static readonly Version MyVersion = new Version(1, 0, 0, 0);

    }
}
