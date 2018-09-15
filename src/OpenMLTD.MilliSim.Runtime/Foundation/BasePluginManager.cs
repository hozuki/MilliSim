using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Core;

namespace OpenMLTD.MilliSim.Foundation {
    /// <inheritdoc />
    /// <summary>
    /// Base plugin manager. This class must be inherited.
    /// </summary>
    public abstract class BasePluginManager : DisposableBase {

        /// <summary>
        /// Gets the list of loaded plugins.
        /// </summary>
        public abstract IReadOnlyList<IMilliSimPlugin> Plugins { get; }

        /// <summary>
        /// Gets a list of plugins whose type is the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the plugin. This is a type that implements <see cref="IMilliSimPlugin"/>.</typeparam>
        /// <returns>The list of satisfied plugins.</returns>
        [NotNull, ItemNotNull]
        public IReadOnlyList<T> GetPluginsOfType<T>()
            where T : class, IMilliSimPlugin {
            var t = typeof(T);

            if (_typedPlugins.ContainsKey(t)) {
                return (IReadOnlyList<T>)_typedPlugins[t];
            }

            var list = Plugins.OfType<T>().ToArray();

            _typedPlugins.Add(t, list);

            return list;
        }

        /// <summary>
        /// Gets the only plugin whose ID is the specified ID and type is the specified type.
        /// </summary>
        /// <returns>The first satisfied plugin.</returns>
        /// <exception cref="InvalidOperationException">There are more than one plugins sharing the same ID.</exception>
        [CanBeNull]
        public IMilliSimPlugin GetPluginByID([NotNull] string id) {
            try {
                var result = Plugins.SingleOrDefault(plugin => plugin.PluginID == id);
                return result;
            } catch (InvalidOperationException) {
                return null;
            }
        }

        /// <summary>
        /// Gets the only plugin whose ID is the specified ID and type is the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the plugin. This is a type that implements <see cref="IMilliSimPlugin"/>.</typeparam>
        /// <returns>The first satisfied plugin.</returns>
        /// <exception cref="InvalidOperationException">There are more than one plugins sharing the same ID.</exception>
        [CanBeNull]
        public T GetPluginByID<T>([NotNull] string id)
            where T : class, IMilliSimPlugin {
            var l = GetPluginsOfType<T>();

            return l.SingleOrDefault(plugin => plugin.PluginID == id);
        }

        /// <summary>
        /// Gets the list of plugins whose category is the specified category and type is the specified type.
        /// </summary>
        /// <returns>The list of satisfied plugins.</returns>
        [NotNull, ItemNotNull]
        public IReadOnlyList<IMilliSimPlugin> GetPluginsByCategory([NotNull] string category) {
            return Plugins.Where(plugin => plugin.PluginCategory == category).ToArray();
        }

        /// <summary>
        /// Gets the list of plugins whose category is the specified category and type is the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the plugin. This is a type that implements <see cref="IMilliSimPlugin"/>.</typeparam>
        /// <returns>The list of satisfied plugins.</returns>
        [NotNull, ItemNotNull]
        public IReadOnlyList<T> GetPluginsByCategory<T>([NotNull] string category) where T : class, IMilliSimPlugin {
            return Plugins.OfType<T>().Where(plugin => plugin.PluginCategory == category).ToArray();
        }

        private readonly Dictionary<Type, IReadOnlyList<IMilliSimPlugin>> _typedPlugins = new Dictionary<Type, IReadOnlyList<IMilliSimPlugin>>();

    }
}
