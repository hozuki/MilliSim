using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Plugin;

namespace OpenMLTD.MilliSim.Foundation {
    public abstract class BasePluginManager : DisposableBase {

        public abstract IReadOnlyList<IMilliSimPlugin> Plugins { get; }

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

        [CanBeNull]
        public IMilliSimPlugin GetPluginByID([NotNull] string id) {
            try {
                var result = Plugins.SingleOrDefault(plugin => plugin.PluginID == id);
                return result;
            } catch (InvalidOperationException) {
                return null;
            }
        }

        [CanBeNull]
        public T GetPluginByID<T>([NotNull] string id)
            where T : class, IMilliSimPlugin {
            var l = GetPluginsOfType<T>();

            return l.SingleOrDefault(plugin => plugin.PluginID == id);
        }

        [NotNull, ItemNotNull]
        public IReadOnlyList<IMilliSimPlugin> GetPluginsByCategory([NotNull] string category) {
            return Plugins.Where(plugin => plugin.PluginCategory == category).ToArray();
        }

        [NotNull, ItemNotNull]
        public IReadOnlyList<T> GetPluginsByCategory<T>([NotNull] string category) where T : class, IMilliSimPlugin {
            return Plugins.OfType<T>().Where(plugin => plugin.PluginCategory == category).ToArray();
        }

        private readonly Dictionary<Type, IReadOnlyList<IMilliSimPlugin>> _typedPlugins = new Dictionary<Type, IReadOnlyList<IMilliSimPlugin>>();

    }
}
