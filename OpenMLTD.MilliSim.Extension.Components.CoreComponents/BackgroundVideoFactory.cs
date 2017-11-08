using System;
using System.Diagnostics;
using System.IO;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Extension.Components.CoreComponents.Configuration;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Graphics;

namespace OpenMLTD.MilliSim.Extension.Components.CoreComponents {
    [MilliSimPlugin(typeof(IComponentFactory))]
    public sealed class BackgroundVideoFactory : ComponentFactory {

        public override string PluginID => "plugin.component_factory.background_video";

        public override string PluginName => "BackgroundVideo Component Factory";

        public override string PluginDescription => "BackgroundVideo Component Factory";

        public override string PluginAuthor => "OpenMLTD";

        public override Version PluginVersion => MyVersion;

        public override IComponent CreateComponent(GameBase game, IComponentContainer parent) {
            var config = game.ConfigurationStore.Get<BackgroundVideoConfig>();
            if (!string.IsNullOrEmpty(config.Data.BackgroundVideo) && File.Exists(config.Data.BackgroundVideo)) {
                Trace.Assert(parent is IVisualContainer);
                return new BackgroundVideo((IVisualContainer)parent);
            } else {
                return null;
            }
        }

        private static readonly Version MyVersion = new Version(1, 0, 0, 0);

    }
}
