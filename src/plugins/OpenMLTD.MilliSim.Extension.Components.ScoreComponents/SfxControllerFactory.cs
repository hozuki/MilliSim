using System;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Foundation.Extending;

namespace OpenMLTD.MilliSim.Extension.Components.ScoreComponents {
    [MilliSimPlugin(typeof(IBaseGameComponentFactory))]
    public sealed class SfxControllerFactory : BaseGameComponentFactory {

        public override string PluginID => "plugin.component_factory.sfx_controller";

        public override string PluginName => "SfxController Component Factory";

        public override string PluginDescription => "SfxController Component Factory";

        public override string PluginAuthor => "OpenMLTD";

        public override Version PluginVersion => MyVersion;

        public override IBaseGameComponent CreateComponent(BaseGame game, IBaseGameComponentContainer parent) {
            return new SfxController(game, parent);
        }

        private static readonly Version MyVersion = new Version(1, 0, 0, 0);

    }
}
