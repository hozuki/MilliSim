using System;
using OpenMLTD.MilliSim.Core;

namespace OpenMLTD.MilliSim.Foundation {
    public abstract class ComponentFactory : DisposableBase, IComponentFactory {

        public int ApiVersion => 1;

        public string PluginCategory => "Component Factory";

        public abstract string PluginID { get; }
        public abstract string PluginName { get; }
        public abstract string PluginDescription { get; }
        public abstract string PluginAuthor { get; }
        public abstract Version PluginVersion { get; }

        public abstract IComponent CreateComponent(GameBase game, IComponentContainer parent);

        protected override void Dispose(bool disposing) {
        }

    }
}
