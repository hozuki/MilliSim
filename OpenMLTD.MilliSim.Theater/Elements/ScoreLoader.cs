using System;
using System.Diagnostics;
using System.IO;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Core.Entities;
using OpenMLTD.MilliSim.Core.Entities.Runtime;
using OpenMLTD.MilliSim.Core.Entities.Runtime.Extensions;
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

            if (Program.ScoreReaders.Count == 0) {
                if (debug != null) {
                    debug.AddLine("ERROR: No available score reader.");
                }
                return;
            }

            var successful = false;
            foreach (var reader in Program.ScoreReaders) {
                if (reader.SupportsFileType(scoreFileName)) {
                    using (var fileStream = File.Open(scoreFileName, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                        successful = reader.TryRead(fileStream, fileStream.Name, out var score);
                        if (successful) {
                            _score = score;
                            break;
                        } else {
                            if (debug != null) {
                                debug.AddLine($"Warning: Unable to read score using reader <{reader.Description}>. Continue searching.");
                            }
                        }
                    }
                }
            }

            if (_score != null) {
                try {
                    var options = new ScoreCompileOptions {
                        Difficulty = settings.Game.Difficulty
                    };
                    RuntimeScore = _score.Compile(options);
                } catch (Exception ex) {
                    successful = false;
                    if (debug != null) {
                        var siteInfo = ex.TargetSite.DeclaringType != null ?
                            $"{ex.TargetSite.DeclaringType.Name}:{ex.TargetSite.Name}" :
                            ex.TargetSite.Name;
                        debug.AddLine($"ERROR: {ex.Message} @ {siteInfo}");
                    }
                    Debug.Print(ex.StackTrace);
                }
            }

            if (!successful) {
                if (debug != null) {
                    debug.AddLine($"ERROR: No score reader can read score file <{scoreFileName}>.");
                }
            } else {
                if (debug != null) {
                    debug.AddLine($"Loaded score file: {scoreFileName}");
                }
            }
        }

        private Score _score;

    }
}
