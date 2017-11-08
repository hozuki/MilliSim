using System;
using System.Diagnostics;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Graphics;

namespace OpenMLTD.MilliSim.Extension.Components.CoreComponents {
    [MilliSimPlugin(typeof(IComponentFactory))]
    public sealed class HelpOverlayFactory : ComponentFactory {

        public override string PluginID => "plugin.component_factory.help_overlay";

        public override string PluginName => "HelpOverlay Component Factory";

        public override string PluginDescription => "HelpOverlay Component Factory";

        public override string PluginAuthor => "OpenMLTD";

        public override Version PluginVersion => MyVersion;

        public override IComponent CreateComponent(GameBase game, IComponentContainer parent) {
            Trace.Assert(parent is IVisualContainer);

            var help = new HelpOverlay((IVisualContainer)parent);
            help.Visible = false;
            help.Text = "Press space to start";

            return help;
        }

        private static readonly Version MyVersion = new Version(1, 0, 0, 0);

    }
}
