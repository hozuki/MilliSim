using System;
using System.Diagnostics;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Graphics;

namespace OpenMLTD.MilliSim.Extension.Components.ExtraComponents {
    [MilliSimPlugin(typeof(IComponentFactory))]
    public sealed class MiniCubeFactory : ComponentFactory {

        public override string PluginID => "plugin.component_factory.mini_cube";

        public override string PluginName => "MiniCube Component Factory";

        public override string PluginDescription => "MiniCube Component Factory";

        public override string PluginAuthor => "OpenMLTD";

        public override Version PluginVersion => MyVersion;

        public override IComponent CreateComponent(GameBase game, IComponentContainer parent) {
            Trace.Assert(parent is IVisualContainer);
            return new MiniCube((IVisualContainer)parent);
        }

        private static readonly Version MyVersion = new Version(1, 0, 0, 0);

    }
}
