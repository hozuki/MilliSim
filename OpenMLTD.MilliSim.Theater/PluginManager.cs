using System;
using System.Collections.Generic;
using System.Composition.Hosting;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Audio.Extending;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Core.Entities.Extending;

namespace OpenMLTD.MilliSim.Theater {
    public sealed class PluginManager : DisposableBase {

        internal PluginManager([ItemNotNull] params string[] directories) {
            if (IsInitialized) {
                return;
            }

            var allAssemblies = new List<Assembly>();
            foreach (var directory in directories) {
                if (!Directory.Exists(directory)) {
                    continue;
                }

                var assemblyFileNames = Directory
                    .EnumerateFiles(directory)
                    .Where(str => str.ToLowerInvariant().EndsWith(".dll"));

                foreach (var assemblyFileName in assemblyFileNames) {
                    try {
                        var assembly = Assembly.LoadFrom(assemblyFileName);
                        allAssemblies.Add(assembly);
                    } catch (Exception ex) {
                        Debug.Print(ex.Message);
                    }
                }
            }

            var configuration = new ContainerConfiguration().WithAssemblies(allAssemblies);
            var host = configuration.CreateContainer();

            ScoreFormats = host.GetExports<IScoreFormat>().ToArray();
            AudioFormats = host.GetExports<IAudioFormat>().ToArray();

            IsInitialized = true;

            _extensionConfiguration = configuration;
            _extensionContainer = host;
        }

        public bool IsInitialized { get; private set; }

        public IReadOnlyList<IScoreFormat> ScoreFormats {
            get;
            [UsedImplicitly]
            private set;
        }

        public IReadOnlyList<IAudioFormat> AudioFormats {
            get;
            [UsedImplicitly]
            private set;
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                _extensionContainer.Dispose();
            }
        }

        private readonly ContainerConfiguration _extensionConfiguration;
        private readonly CompositionHost _extensionContainer;

    }
}
