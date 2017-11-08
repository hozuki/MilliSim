using System;
using System.Diagnostics;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Graphics;

namespace OpenMLTD.MilliSim.Extension.Components.CoreComponents {
    [MilliSimPlugin(typeof(IComponentFactory))]
    public sealed class TextOverlayFactory : ComponentFactory {

        public override string PluginID => "plugin.component_factory.text_overlay";

        public override string PluginName => "TextOverlay Component Factory";

        public override string PluginDescription => "TextOverlay Component Factory";

        public override string PluginAuthor => "OpenMLTD";

        public override Version PluginVersion => MyVersion;

        public override IComponent CreateComponent(GameBase game, IComponentContainer parent) {
            Trace.Assert(parent is IVisualContainer);
            return new TextOverlay((IVisualContainer)parent);
        }

        private static readonly Version MyVersion = new Version(1, 0, 0, 0);

    }
}
