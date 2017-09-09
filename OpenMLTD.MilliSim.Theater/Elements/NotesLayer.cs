using System;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Core.Entities;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Graphics;
using OpenMLTD.MilliSim.Graphics.Drawing;
using OpenMLTD.MilliSim.Graphics.Drawing.Direct2D.Advanced;
using OpenMLTD.MilliSim.Graphics.Extensions;
using OpenMLTD.MilliSim.Theater.Extensions;

namespace OpenMLTD.MilliSim.Theater.Elements {
    public class NotesLayer : BufferedElement2D {

        public NotesLayer(GameBase game)
            : base(game) {
        }

        /// <summary>
        /// Note falling speed scale. The same one used in game.
        /// </summary>
        public float SpeedScale {
            get => _speedScale;
            set {
                if (value <= 0.05f) {
                    value = 0.05f;
                }
                _speedScale = value;
            }
        }

        protected override void OnDrawBuffer(GameTime gameTime, RenderContext context) {
            base.OnDrawBuffer(gameTime, context);

            if (_score == null) {
                return;
            }

            if (_noteImages?[0] == null) {
                return;
            }

            var noteImages = _noteImages;
            var activeNotes = _activeNotes;

            if (activeNotes == null) {
                return;
            }

            var theaterDays = Game.AsTheaterDays();

            var syncTimer = theaterDays.FindSingleElement<SyncTimer>();
            if (syncTimer == null) {
                return;
            }

            var tapPoints = theaterDays.FindSingleElement<TapPoints>();
            if (tapPoints == null) {
                throw new InvalidOperationException();
            }

            var settings = Program.Settings;
            var difficulty = settings.Game.Difficulty;
            var scaledNoteSize = settings.Scaling.Note;
            var currentSecond = syncTimer.CurrentTime.TotalSeconds;
            var speedScale = SpeedScale;
            var trackType = NoteHelper.MapDifficultyToTrackType(difficulty);
            var trackIndices = NoteHelper.GetTrackIndicesFromTrackType(trackType);
            var trackXRatios = tapPoints.TapPointXRatios;
            var tapPointsLayout = settings.UI.TapPoints.Layout;
            var notesLayerLayout = settings.UI.NotesLayer.Layout;
            var clientSize = context.ClientSize;

            var startY = notesLayerLayout.Y * clientSize.Height;
            var endY = tapPointsLayout.Y * clientSize.Height;

            context.Begin2D();

            for (var i = 0; i < activeNotes.Length; ++i) {
                var note = activeNotes[i];
                var leadTimeScaled = (float)note.LeadTime / (speedScale * speedScale);
                var noteEnterTime = note.AbsoluteTime - leadTimeScaled;
                var noteLeaveTime = note.AbsoluteTime;

                if (!(noteEnterTime <= currentSecond && currentSecond <= noteLeaveTime)) {
                    continue;
                }

                var imageIndex = -1;
                switch (note.Type) {
                    case NoteType.TapSmall:
                        imageIndex = 0;
                        break;
                    case NoteType.TapLarge:
                        imageIndex = 1;
                        break;
                    case NoteType.FlickLeft:
                        imageIndex = 4;
                        break;
                    case NoteType.FlickUp:
                        imageIndex = 5;
                        break;
                    case NoteType.FlickRight:
                        imageIndex = 6;
                        break;
                    case NoteType.HoldSmall:
                        imageIndex = 2;
                        break;
                    case NoteType.SlideSmall:
                        imageIndex = 7;
                        break;
                    case NoteType.HoldLarge:
                        imageIndex = 3;
                        break;
                    case NoteType.Special:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (imageIndex < 0) {
                    continue;
                }

                var indexX = Array.IndexOf(trackIndices, note.TrackIndex);
                var x = clientSize.Width * trackXRatios[indexX];
                var y = MathHelper.Lerp(startY, endY, (float)(currentSecond - noteEnterTime) / leadTimeScaled);

                context.DrawImageStrip(noteImages[0], imageIndex, x - scaledNoteSize.Width / 2, y - scaledNoteSize.Height / 2, scaledNoteSize.Width, scaledNoteSize.Height);
            }

            //if (_noteImages?[0] != null) {
            //    var strip = _noteImages[0];
            //    context.DrawImageStrip(strip, 1, 130, 340, scaledNoteSize.Width, scaledNoteSize.Height);
            //}
            context.End2D();
        }

        protected override void OnGotContext(RenderContext context) {
            base.OnGotContext(context);

            var settings = Program.Settings;
            var theaterDays = Game.AsTheaterDays();
            var debugOverlay = theaterDays.FindSingleElement<DebugOverlay>();

            if (settings.Images.Notes.Length == 0) {
                if (debugOverlay != null) {
                    debugOverlay.AddLine("WARNING: default notes image strip is not specified.");
                }
            } else {
                _noteImages = new D2DImageStrip[settings.Images.Notes.Length];

                for (var i = 0; i < settings.Images.Notes.Length; ++i) {
                    var notesImageEntry = settings.Images.Notes[i];
                    if (notesImageEntry == null) {
                        continue;
                    }

                    if (notesImageEntry.File == null || !File.Exists(notesImageEntry.File)) {
                        if (i == 0) {
                            if (debugOverlay != null) {
                                debugOverlay.AddLine($"WARNING: default notes image strip <{notesImageEntry.File ?? string.Empty}> is not found.");
                            }
                        } else {
                            if (debugOverlay != null) {
                                debugOverlay.AddLine($"WARNING: notes image strip <{notesImageEntry.File ?? string.Empty}> is not found, falling back to default.");
                            }
                        }
                        continue;
                    }

                    var imageStrip = Direct2DHelper.LoadImageStrip(context, notesImageEntry.File, notesImageEntry.Count, notesImageEntry.Orientation);
                    _noteImages[i] = imageStrip;
                }
            }

            Opacity = settings.UI.NotesLayer.Opacity;
        }

        protected override void OnLostContext(RenderContext context) {
            base.OnLostContext(context);

            if (_noteImages != null) {
                foreach (var noteImage in _noteImages) {
                    noteImage?.Dispose();
                }
            }
            _noteImages = null;
        }

        protected override void OnInitialize() {
            base.OnInitialize();

            var scoreLoader = Game.AsTheaterDays().FindSingleElement<ScoreLoader>();
            _score = scoreLoader?.Score;

            if (_score != null) {
                var notes = _score.Notes;
                if (notes == null) {
                    return;
                }

                var settings = Program.Settings;
                var difficulty = settings.Game.Difficulty;

                var trackType = NoteHelper.MapDifficultyToTrackType(difficulty);
                var trackIndices = NoteHelper.GetTrackIndicesFromTrackType(trackType);
                _activeNotes = notes.Where(n => Array.IndexOf(trackIndices, n.TrackIndex) >= 0).ToArray();
            }
        }

        private D2DImageStrip[] _noteImages;
        [CanBeNull]
        private Score _score;
        [CanBeNull]
        private Note[] _activeNotes;

        private float _speedScale = 1f;

    }
}
