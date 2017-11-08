using System;
using System.Diagnostics;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Graphics;

namespace OpenMLTD.MilliSim.Extension.Components.ExtraComponents {
    [MilliSimPlugin(typeof(IComponentFactory))]
    public sealed class CuteIdolFactory : ComponentFactory {

        public override string PluginID => "plugin.component_factory.cute_idol";

        public override string PluginName => "CuteIdol Component Factory";

        public override string PluginDescription => "CuteIdol Component Factory";

        public override string PluginAuthor => "OpenMLTD";

        public override Version PluginVersion => MyVersion;

        public override IComponent CreateComponent(GameBase game, IComponentContainer parent) {
            Trace.Assert(parent is IVisualContainer);
            return new CuteIdol((IVisualContainer)parent);
        }

        private static readonly Version MyVersion = new Version(1, 0, 0, 0);

    }
}
