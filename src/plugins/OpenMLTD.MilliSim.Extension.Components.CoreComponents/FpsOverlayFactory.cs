using System;
using System.Diagnostics;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Extension.Components.CoreComponents.Configuration;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Graphics;
using OpenMLTD.MilliSim.Plugin;

namespace OpenMLTD.MilliSim.Extension.Components.CoreComponents {
    [MilliSimPlugin(typeof(IBaseGameComponentFactory))]
    public sealed class FpsOverlayFactory : BaseGameComponentFactory {

        public override string PluginID => "plugin.component_factory.fps_overlay";

        public override string PluginName => "FpsOverlay Component Factory";

        public override string PluginDescription => "FpsOverlay Component Factory";

        public override string PluginAuthor => "OpenMLTD";

        public override Version PluginVersion => MyVersion;

        public override IBaseGameComponent CreateComponent(BaseGame game, IBaseGameComponentContainer parent) {
            var config = game.ConfigurationStore.Get<FpsOverlayConfig>();
            if (config.Data.Visible) {
                Trace.Assert(parent is IVisualContainer);
                var fps = new FpsOverlay(game, (IVisualContainer)parent);
                fps.FillColor = config.Data.TextFill;
                fps.FontSize = config.Data.FontSize;
                return fps;
            } else {
                return null;
            }
        }

        private static readonly Version MyVersion = new Version(1, 0, 0, 0);

    }
}
