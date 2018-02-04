using System;
using System.Diagnostics;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Graphics;
using OpenMLTD.MilliSim.Plugin;

namespace OpenMLTD.MilliSim.Extension.Components.CoreComponents {
    [MilliSimPlugin(typeof(IBaseGameComponentFactory))]
    public sealed class HelpOverlayFactory : BaseGameComponentFactory {

        public override string PluginID => "plugin.component_factory.help_overlay";

        public override string PluginName => "HelpOverlay Component Factory";

        public override string PluginDescription => "HelpOverlay Component Factory";

        public override string PluginAuthor => "OpenMLTD";

        public override Version PluginVersion => MyVersion;

        public override IBaseGameComponent CreateComponent(BaseGame game, IBaseGameComponentContainer parent) {
            Trace.Assert(parent is IVisualContainer);

            var translationManager = game.CultureSpecificInfo.TranslationManager;

            var help = new HelpOverlay(game, (IVisualContainer)parent);
            help.Visible = true;
            var helpText = translationManager.Get("system_ui.help.press_space_to_start");
            help.Text = helpText.Length > 0 ? helpText : "Press space to start";

            return help;
        }

        private static readonly Version MyVersion = new Version(1, 0, 0, 0);

    }
}
