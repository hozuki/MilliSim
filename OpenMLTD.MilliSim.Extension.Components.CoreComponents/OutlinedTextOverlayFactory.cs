using System;
using System.Diagnostics;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Graphics;

namespace OpenMLTD.MilliSim.Extension.Components.CoreComponents {
    [MilliSimPlugin(typeof(IComponentFactory))]
    public sealed class OutlinedTextOverlayFactory : ComponentFactory {

        public override string PluginID => "plugin.component_factory.outlined_text_overlay";

        public override string PluginName => "OutlinedTextOverlay Component Factory";

        public override string PluginDescription => "OutlinedTextOverlay Component Factory";

        public override string PluginAuthor => "OpenMLTD";

        public override Version PluginVersion => MyVersion;

        public override IComponent CreateComponent(GameBase game, IComponentContainer parent) {
            Trace.Assert(parent is IVisualContainer);
            return new OutlinedTextOverlay((IVisualContainer)parent);
        }

        private static readonly Version MyVersion = new Version(1, 0, 0, 0);

    }
}
