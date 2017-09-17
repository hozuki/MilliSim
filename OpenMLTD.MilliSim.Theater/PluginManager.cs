using System;
using System.Collections.Generic;
using System.Composition.Hosting;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Audio.Extending;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Core.Entities.Extending;
using OpenMLTD.MilliSim.Theater.Animation.Extending;

namespace OpenMLTD.MilliSim.Theater {
    public sealed class PluginManager : DisposableBase {

        internal PluginManager([ItemNotNull] params string[] directories) {
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

            var scoreFormats = host.GetExports<IScoreFormat>().ToArray();
            var audioFormats = host.GetExports<IAudioFormat>().ToArray();
            var noteTraceCalculators = host.GetExports<INoteTraceCalculator>().ToArray();
            var loadedPlugins = Enumerable.Empty<IMilliSimPlugin>()
                .Concat(scoreFormats)
                .Concat(audioFormats)
                .Concat(noteTraceCalculators)
                .ToArray();

            ScoreFormats = scoreFormats;
            AudioFormats = audioFormats;
            NoteTraceCalculators = noteTraceCalculators;
            LoadedPlugins = loadedPlugins;

            _extensionConfiguration = configuration;
            _extensionContainer = host;
        }

        public IReadOnlyList<IScoreFormat> ScoreFormats { get; }

        public IReadOnlyList<IAudioFormat> AudioFormats { get; }

        public IReadOnlyList<INoteTraceCalculator> NoteTraceCalculators { get; }

        /// <summary>
        /// Can be used in diagnostic windows, for example a <see cref="ListView"/> listing plugin details.
        /// </summary>
        internal IReadOnlyList<IMilliSimPlugin> LoadedPlugins { get; }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                _extensionContainer.Dispose();
            }
        }

        private readonly ContainerConfiguration _extensionConfiguration;
        private readonly CompositionHost _extensionContainer;

    }
}
