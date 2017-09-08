using System;
using System.IO;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Core.Entities;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Theater.Extensions;

namespace OpenMLTD.MilliSim.Theater.Elements {
    public sealed class ScoreLoader : Element {

        public ScoreLoader(GameBase game)
            : base(game) {
        }

        [CanBeNull]
        public Score Score { get; private set; }

        protected override void OnInitialize() {
            base.OnInitialize();

            var settings = Program.Settings;
            var scoreFileName = settings.Game.ScoreFile;
            var debug = Game.AsTheaterDays().GetSingleElement<DebugOverlay>();

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
                            Score = score;
                            break;
                        } else {
                            if (debug != null) {
                                debug.AddLine($"Warning: Unable to read score using reader <{reader.Description}>. Continue searching.");
                            }
                        }
                    }
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

    }
}
