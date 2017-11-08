using System;
using System.Collections.Generic;
using System.Composition.Hosting;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Core;

namespace OpenMLTD.MilliSim.GameAbstraction {
    public sealed class PluginManager : DisposableBase {

        internal PluginManager() {
        }

        [NotNull, ItemNotNull]
        public IReadOnlyList<T> GetPluginsOfType<T>() where T : class, IMilliSimPlugin {
            var t = typeof(T);
            if (_typedPlugins.ContainsKey(t)) {
                return (IReadOnlyList<T>)_typedPlugins[t];
            }

            var result = LoadedPlugins.OfType<T>().ToArray();
            _typedPlugins[t] = result;

            return result;
        }

        [CanBeNull]
        public IMilliSimPlugin GetPluginByID([NotNull] string id) {
            try {
                var result = LoadedPlugins.SingleOrDefault(plugin => plugin.PluginID == id);
                return result;
            } catch (InvalidOperationException) {
                return null;
            }
        }

        [NotNull, ItemNotNull]
        public IReadOnlyList<IMilliSimPlugin> GetPluginsByCategory([NotNull] string category) {
            return LoadedPlugins.Where(plugin => plugin.PluginCategory == category).ToArray();
        }

        [NotNull, ItemNotNull]
        public IReadOnlyList<T> GetPluginsByCategory<T>([NotNull] string category) where T : class, IMilliSimPlugin {
            return LoadedPlugins.OfType<T>().Where(plugin => plugin.PluginCategory == category).ToArray();
        }

        /// <summary>
        /// Can be used in diagnostic windows, for example a <see cref="System.Windows.Forms.ListView"/> listing plugin details.
        /// </summary>
        public IReadOnlyList<IMilliSimPlugin> LoadedPlugins { get; private set; }

        internal void LoadAssemblies(PluginSearchingMode searchingMode, [CanBeNull, ItemNotNull] string[] filters, [NotNull, ItemNotNull] params string[] paths) {
            if (_extensionContainer != null) {
                throw new InvalidOperationException();
            }

            var allAssemblies = FindAssemblies(searchingMode, filters, paths);
            var configuration = new ContainerConfiguration().WithAssemblies(allAssemblies);
            var host = configuration.CreateContainer();

            var loadedPlugins = host.GetExports<IMilliSimPlugin>().ToArray();
            LoadedPlugins = loadedPlugins;

            _extensionConfiguration = configuration;
            _extensionContainer = host;
        }

        protected override void Dispose(bool disposing) {
            if (!disposing) {
                return;
            }

            _extensionContainer.Dispose();

            foreach (var plugin in LoadedPlugins) {
                if (plugin is IDisposable d) {
                    d.Dispose();
                }
            }
        }

        private static IReadOnlyList<Assembly> FindAssemblies(PluginSearchingMode searchingMode, [CanBeNull, ItemNotNull] string[] filters, [NotNull, ItemNotNull] params string[] paths) {
            var allAssemblies = new List<Assembly>();
            var fullFilters = filters?.Where(f => !string.IsNullOrEmpty(f)).Select(Path.GetFullPath).ToArray() ?? new string[0];

            var isModernWindows = Environment.OSVersion.Platform == PlatformID.Win32NT;

            foreach (var directory in paths) {
                if (!Directory.Exists(directory)) {
                    continue;
                }

                var assemblyFileNames = Directory
                    .EnumerateFiles(directory)
                    .Where(str => str.ToLowerInvariant().EndsWith(".dll"));

                foreach (var assemblyFileName in assemblyFileNames) {
                    bool PathEquals(string filter) {
                        var compareFlag = isModernWindows ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
                        return string.Equals(filter, assemblyFileName, compareFlag);
                    }

                    if (fullFilters.Length > 0) {
                        switch (searchingMode) {
                            case PluginSearchingMode.Default:
                                break;
                            case PluginSearchingMode.Exclusive:
                                if (Array.Exists(fullFilters, PathEquals)) {
                                    continue;
                                }
                                break;
                            case PluginSearchingMode.Inclusive:
                                if (!Array.Exists(fullFilters, PathEquals)) {
                                    continue;
                                }
                                break;
                            default:
                                throw new ArgumentOutOfRangeException(nameof(searchingMode), searchingMode, null);
                        }
                    }

                    try {
                        var assembly = Assembly.LoadFrom(assemblyFileName);
                        allAssemblies.Add(assembly);
                    } catch (Exception ex) {
                        Debug.Print(ex.Message);
                    }
                }
            }

            return allAssemblies;
        }

        private readonly Dictionary<Type, IReadOnlyList<IMilliSimPlugin>> _typedPlugins = new Dictionary<Type, IReadOnlyList<IMilliSimPlugin>>();
        private ContainerConfiguration _extensionConfiguration;
        private CompositionHost _extensionContainer;

    }
}
