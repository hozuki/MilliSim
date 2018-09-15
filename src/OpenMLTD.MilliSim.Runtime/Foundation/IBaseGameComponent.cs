using System;
using Microsoft.Xna.Framework;
using OpenMLTD.MilliSim.Configuration;

namespace OpenMLTD.MilliSim.Foundation {
    /// <inheritdoc cref="IBaseGameComponent"/>
    /// <inheritdoc cref="IDisposable"/>
    /// <inheritdoc cref="IUpdateable"/>
    /// <summary>
    /// An interface for base game components.
    /// </summary>
    public interface IBaseGameComponent : IGameComponent, IDisposable, IUpdateable {

        /// <summary>
        /// Gets the name of this component.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets or sets the parent of this component.
        /// </summary>
        IBaseGameComponentContainer Parent { get; set; }

        /// <summary>
        /// Gets the <see cref="OpenMLTD.MilliSim.Configuration.BaseConfigurationStore"/> used by this component.
        /// </summary>
        BaseConfigurationStore ConfigurationStore { get; }

        /// <summary>
        /// Load associated contents.
        /// </summary>
        void LoadContents();

        /// <summary>
        /// Unload associated contents.
        /// </summary>
        void UnloadContents();

        /// <summary>
        /// Gets a <see cref="bool"/> indicating whether this component's contents are loaded.
        /// </summary>
        bool AreContentsLoaded { get; }

    }
}
