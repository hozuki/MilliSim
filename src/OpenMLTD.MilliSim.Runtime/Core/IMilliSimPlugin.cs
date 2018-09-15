using System;
using JetBrains.Annotations;

namespace OpenMLTD.MilliSim.Core {
    /// <inheritdoc />
    /// <summary>
    /// Core interface for MilliSim plugins.
    /// </summary>
    public interface IMilliSimPlugin : IApiVersionProvider {

        /// <summary>
        /// Gets the plugin ID.
        /// </summary>
        /// <remarks>You can use <see cref="Guid.NewGuid"/> for a unique ID.</remarks>
        [NotNull]
        string PluginID { get; }

        /// <summary>
        /// Gets the plugin name.
        /// </summary>
        [NotNull]
        string PluginName { get; }

        /// <summary>
        /// Gets the plugin description.
        /// </summary>
        [NotNull]
        string PluginDescription { get; }

        /// <summary>
        /// Gets the plugin author name.
        /// </summary>
        [NotNull]
        string PluginAuthor { get; }

        /// <summary>
        /// Gets the plugin version.
        /// </summary>
        [NotNull]
        Version PluginVersion { get; }

        /// <summary>
        /// Gets the plugin category.
        /// </summary>
        [NotNull]
        string PluginCategory { get; }

    }
}
