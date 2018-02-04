using System;
using System.Diagnostics;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Graphics;
using OpenMLTD.MilliSim.Plugin;

namespace OpenMLTD.MilliSim.Extension.Components.CoreComponents {
    [MilliSimPlugin(typeof(IBaseGameComponentFactory))]
    public sealed class TextOverlayFactory : BaseGameComponentFactory {

        public override string PluginID => "plugin.component_factory.text_overlay";

        public override string PluginName => "TextOverlay Component Factory";

        public override string PluginDescription => "TextOverlay Component Factory";

        public override string PluginAuthor => "OpenMLTD";

        public override Version PluginVersion => MyVersion;

        public override IBaseGameComponent CreateComponent(BaseGame game, IBaseGameComponentContainer parent) {
            Trace.Assert(parent is IVisualContainer);
            return new TextOverlay(game, (IVisualContainer)parent);
        }

        private static readonly Version MyVersion = new Version(1, 0, 0, 0);

    }
}
