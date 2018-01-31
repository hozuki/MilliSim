using System;
using System.IO;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Core.Entities.Extending;
using OpenMLTD.MilliSim.Core.Entities.Runtime;
using OpenMLTD.MilliSim.Core.Entities.Source;
using OpenMLTD.MilliSim.Extension.Components.CoreComponents;
using OpenMLTD.MilliSim.Extension.Components.ScoreComponents.Configuration;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.GameAbstraction.Extensions;

namespace OpenMLTD.MilliSim.Extension.Components.ScoreComponents {
    public sealed class ScoreLoader : Component {

        public ScoreLoader([NotNull] IComponentContainer parent)
            : base(parent) {
        }

        [CanBeNull]
        public RuntimeScore RuntimeScore { get; private set; }

        protected override void OnInitialize() {
            base.OnInitialize();

            var config = ConfigurationStore.Get<ScoreLoaderConfig>();
            var scoreFilePath = config.Data.ScoreFilePath;
            var debug = Game.AsTheaterDays().FindSingleElement<DebugOverlay>();

            if (string.IsNullOrEmpty(scoreFilePath)) {
                if (debug != null) {
                    debug.AddLine("ERROR: Score file is not specified.");
                }
                return;
            }

            if (!File.Exists(scoreFilePath)) {
                if (debug != null) {
                    debug.AddLine($"ERROR: Score file <{scoreFilePath}> is missing.");
                }
                return;
            }

            var theaterDays = Game.AsTheaterDays();

            var scoreFormats = theaterDays.PluginManager.GetPluginsOfType<IScoreFormat>();
            if (scoreFormats.Count == 0) {
                if (debug != null) {
                    debug.AddLine("ERROR: No available score reader.");
                }
                return;
            }

            var sourceOptions = new ReadSourceOptions {
                ScoreIndex = config.Data.ScoreIndex
            };
            var compileOptions = new ScoreCompileOptions {
                GlobalSpeed = 1,
                Offset = config.Data.ScoreOffset
            };

            var successful = false;
            RuntimeScore runtimeScore = null;
            SourceScore sourceScore = null;
            foreach (var format in scoreFormats) {
                if (!format.SupportsReadingFileType(scoreFilePath)) {
                    continue;
                }

                using (var reader = format.CreateReader()) {
                    using (var fileStream = File.Open(scoreFilePath, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                        if (!successful) {
                            if (format.CanReadAsSource) {
                                try {
                                    sourceScore = reader.ReadSourceScore(fileStream, scoreFilePath, sourceOptions);
                                    if (!format.CanBeCompiled) {
                                        throw new InvalidOperationException("This format must support compiling source score to runtime score.");
                                    }
                                    using (var compiler = format.CreateCompiler()) {
                                        runtimeScore = compiler.Compile(sourceScore, compileOptions);
                                    }
                                    successful = true;
                                } catch (Exception ex) {
                                    if (debug != null) {
                                        debug.AddLine($"An exception is thrown while trying to read the score using <{format.PluginDescription}>: {ex.Message}");
                                        debug.AddLine(ex.StackTrace);
                                    }
                                }
                            }
                        }

                        if (!successful) {
                            if (format.CanReadAsCompiled) {
                                try {
                                    runtimeScore = reader.ReadCompiledScore(fileStream, scoreFilePath, sourceOptions, compileOptions);
                                    successful = true;
                                } catch (Exception ex) {
                                    if (debug != null) {
                                        debug.AddLine($"An exception is thrown while trying to read the score using <{format.PluginDescription}>: {ex.Message}");
                                        debug.AddLine(ex.StackTrace);
                                    }
                                }
                            }
                        }

                        if (successful) {
                            break;
                        }
                    }
                }
            }

            if (!successful) {
                if (debug != null) {
                    debug.AddLine($"ERROR: No score reader can read score file <{scoreFilePath}>.");
                }
            } else {
                _score = sourceScore;
                RuntimeScore = runtimeScore;
                if (debug != null) {
                    debug.AddLine($"Loaded score file: {scoreFilePath}");
                }
            }
        }

        private SourceScore _score;

    }
}
