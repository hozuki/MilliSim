using System;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Foundation.Extending;

namespace OpenMLTD.MilliSim.Foundation {
    /// <inheritdoc cref="DisposableBase"/>
    /// <inheritdoc cref="IBaseGameComponentFactory"/>
    /// <summary>
    /// The base class for game component factories.
    /// </summary>
    public abstract class BaseGameComponentFactory : DisposableBase, IBaseGameComponentFactory {

        public int ApiVersion => 1;

        public string PluginCategory => "Base Game Component Factory";

        public abstract string PluginID { get; }

        public abstract string PluginName { get; }

        public abstract string PluginDescription { get; }

        public abstract string PluginAuthor { get; }

        public abstract Version PluginVersion { get; }

        public abstract IBaseGameComponent CreateComponent(BaseGame game, IBaseGameComponentContainer parent);

        protected override void Dispose(bool disposing) {
        }

    }
}
