using System;
using System.Diagnostics;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Graphics;
using OpenMLTD.MilliSim.Plugin;

namespace OpenMLTD.MilliSim.Extension.Components.ExtraComponents {
    [MilliSimPlugin(typeof(IBaseGameComponentFactory))]
    public sealed class MiniCubeFactory : BaseGameComponentFactory {

        public override string PluginID => "plugin.component_factory.mini_cube";

        public override string PluginName => "MiniCube Component Factory";

        public override string PluginDescription => "MiniCube Component Factory";

        public override string PluginAuthor => "OpenMLTD";

        public override Version PluginVersion => MyVersion;

        public override IBaseGameComponent CreateComponent(BaseGame game, IBaseGameComponentContainer parent) {
            Trace.Assert(parent is IVisualContainer);
            return new MiniCube(game, (IVisualContainer)parent);
        }

        private static readonly Version MyVersion = new Version(1, 0, 0, 0);

    }
}
