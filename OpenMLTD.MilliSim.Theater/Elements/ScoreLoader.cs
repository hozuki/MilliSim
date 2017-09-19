using System;
using System.IO;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Core.Entities.Runtime;
using OpenMLTD.MilliSim.Core.Entities.Source;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Theater.Extensions;

namespace OpenMLTD.MilliSim.Theater.Elements {
    public sealed class ScoreLoader : Element {

        public ScoreLoader(GameBase game)
            : base(game) {
        }

        [CanBeNull]
        public RuntimeScore RuntimeScore { get; private set; }

        protected override void OnInitialize() {
            base.OnInitialize();

            var settings = Program.Settings;
            var scoreFileName = settings.Game.ScoreFile;
            var debug = Game.AsTheaterDays().FindSingleElement<DebugOverlay>();

            if (string.IsNullOrEmpty(scoreFileName)) {
                if (debug != null) {
                    debug.AddLine("ERROR: Score file is not specified.");
                }
                return;
            }

            if (!File.Exists(scoreFileName)) {
                if (debug != null) {
                    debug.AddLine($"ERROR: Score file <{scoreFileName}> is missing.");
                }
                return;
            }

            if (Program.PluginManager.ScoreFormats.Count == 0) {
                if (debug != null) {
                    debug.AddLine("ERROR: No available score reader.");
                }
                return;
            }

            var sourceOptions = new ReadSourceOptions {
                ScoreIndex = settings.Game.ScoreIndex
            };
            var compileOptions = new ScoreCompileOptions {
                GlobalSpeed = 1
            };

            var successful = false;
            RuntimeScore runtimeScore = null;
            SourceScore sourceScore = null;
            foreach (var format in Program.PluginManager.ScoreFormats) {
                if (!format.SupportsFileType(scoreFileName)) {
                    continue;
                }

                using (var reader = format.CreateReader()) {
                    using (var fileStream = File.Open(scoreFileName, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                        if (!successful) {
                            if (format.CanReadAsSource) {
                                try {
                                    sourceScore = reader.ReadSourceScore(fileStream, scoreFileName, sourceOptions);
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
                                    runtimeScore = reader.ReadCompiledScore(fileStream, scoreFileName, sourceOptions, compileOptions);
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
                    debug.AddLine($"ERROR: No score reader can read score file <{scoreFileName}>.");
                }
            } else {
                _score = sourceScore;
                RuntimeScore = runtimeScore;
                if (debug != null) {
                    debug.AddLine($"Loaded score file: {scoreFileName}");
                }
            }
        }

        private SourceScore _score;

    }
}
