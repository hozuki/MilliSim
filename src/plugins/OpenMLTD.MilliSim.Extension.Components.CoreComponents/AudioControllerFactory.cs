using System;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Plugin;

namespace OpenMLTD.MilliSim.Extension.Components.CoreComponents {
    [MilliSimPlugin(typeof(IBaseGameComponentFactory))]
    public sealed class AudioControllerFactory : BaseGameComponentFactory {

        public override string PluginID => "plugin.component_factory.audio_controller";

        public override string PluginName => "AudioController Component Factory";

        public override string PluginDescription => "AudioController Component Factory";

        public override string PluginAuthor => "OpenMLTD";

        public override Version PluginVersion => MyVersion;

        public override IBaseGameComponent CreateComponent(BaseGame game, IBaseGameComponentContainer parent) {
            return new AudioController(game, parent);
        }

        private static readonly Version MyVersion = new Version(1, 0, 0, 0);

    }
}
