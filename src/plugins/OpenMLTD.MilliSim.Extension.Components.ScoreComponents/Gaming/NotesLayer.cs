using System;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using OpenMLTD.MilliSim.Contributed.Scores;
using OpenMLTD.MilliSim.Contributed.Scores.Animation;
using OpenMLTD.MilliSim.Contributed.Scores.Extending;
using OpenMLTD.MilliSim.Contributed.Scores.Extensions;
using OpenMLTD.MilliSim.Contributed.Scores.Runtime;
using OpenMLTD.MilliSim.Extension.Components.CoreComponents;
using OpenMLTD.MilliSim.Extension.Components.ScoreComponents.Configuration;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Foundation.Extensions;
using OpenMLTD.MilliSim.Graphics;
using OpenMLTD.MilliSim.Graphics.Advanced;
using OpenMLTD.MilliSim.Graphics.Extensions;

namespace OpenMLTD.MilliSim.Extension.Components.ScoreComponents.Gaming {
    public class NotesLayer : BufferedVisual {

        public NotesLayer([NotNull] BaseGame game, [NotNull] IVisualContainer parent)
            : base(game, parent) {
        }

        /// <summary>
        /// Note falling speed scale. The same one used in game.
        /// </summary>
        public float GlobalSpeedScale {
            get => _globalSpeedScale;
            set {
                if (value <= 0.05f) {
                    value = 0.05f;
                }
                _globalSpeedScale = value;
            }
        }

        internal INoteAnimationCalculator AnimationCalculator { get; private set; }

        internal static (int ImageIndex, bool IsHugeNote) GetImageIndex(NoteType type, NoteSize size, FlickDirection flickDirection, bool isHoldStart, bool isHoldEnd, bool isSlideStart, bool isSlideEnd) {
            if (isHoldStart && isHoldEnd) {
                throw new ArgumentException();
            }
            if (isSlideStart && isSlideEnd) {
                throw new ArgumentException();
            }
            if ((isHoldStart || isHoldEnd) && (isSlideStart || isSlideEnd)) {
                throw new ArgumentException();
            }

            var imageIndex = -1;
            var isHugeNote = false;
            switch (type) {
                case NoteType.Tap:
                    switch (size) {
                        case NoteSize.Small:
                            imageIndex = 0;
                            break;
                        case NoteSize.Large:
                            imageIndex = 1;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                case NoteType.Flick:
                    switch (flickDirection) {
                        case FlickDirection.Left:
                            imageIndex = 4;
                            break;
                        case FlickDirection.Up:
                            imageIndex = 5;
                            break;
                        case FlickDirection.Right:
                            imageIndex = 6;
                            break;
                        case FlickDirection.Down:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                case NoteType.Hold:
                    switch (flickDirection) {
                        case FlickDirection.None:
                        case FlickDirection.Down:
                            switch (size) {
                                case NoteSize.Small:
                                    imageIndex = 2;
                                    break;
                                case NoteSize.Large:
                                    imageIndex = 3;
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                            break;
                        case FlickDirection.Left:
                            imageIndex = 4;
                            break;
                        case FlickDirection.Up:
                            imageIndex = 5;
                            break;
                        case FlickDirection.Right:
                            imageIndex = 6;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                case NoteType.Slide:
                    switch (flickDirection) {
                        case FlickDirection.None:
                        case FlickDirection.Down:
                            if (isSlideStart) {
                                imageIndex = 7;
                            } else if (isSlideEnd) {
                                imageIndex = 9;
                            } else {
                                imageIndex = 8;
                            }
                            break;
                        case FlickDirection.Left:
                            imageIndex = 4;
                            break;
                        case FlickDirection.Up:
                            imageIndex = 5;
                            break;
                        case FlickDirection.Right:
                            imageIndex = 6;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                case NoteType.Special:
                    isHugeNote = true;
                    break;
                case NoteType.SpecialEnd:
                case NoteType.SpecialPrepare:
                case NoteType.ScorePrepare:
                    // We don't draw these notes.
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return (imageIndex, isHugeNote);
        }

        protected override void OnDrawBuffer(GameTime gameTime) {
            base.OnDrawBuffer(gameTime);

            var theaterDays = Game.ToBaseGame();

            var scoreLoader = theaterDays.FindSingleElement<ScoreLoader>();

            var score = scoreLoader?.RuntimeScore;

            if (score == null) {
                return;
            }

            if (_noteImages?[0] == null) {
                return;
            }

            var noteImages = _noteImages;
            var notes = score.Notes;

            var syncTimer = theaterDays.FindSingleElement<SyncTimer>();
            if (syncTimer == null) {
                return;
            }

            var tapPoints = theaterDays.FindSingleElement<TapPoints>();
            if (tapPoints == null) {
                throw new InvalidOperationException();
            }

            var scalingResponder = theaterDays.FindSingleElement<MltdStageScalingResponder>();
            if (scalingResponder == null) {
                throw new InvalidOperationException();
            }

            var now = (float)syncTimer.CurrentTime.TotalSeconds;

            var tapPointsConfig = ConfigurationStore.Get<TapPointsConfig>();
            var notesLayerConfig = ConfigurationStore.Get<NotesLayerConfig>();
            var tapPointsLayout = tapPointsConfig.Data.Layout;
            var notesLayerLayout = notesLayerConfig.Data.Layout;
            var clientSize = theaterDays.GraphicsDevice.Viewport;

            var animationMetrics = new NoteAnimationMetrics {
                GlobalSpeedScale = GlobalSpeedScale,
                Width = clientSize.Width,
                Height = clientSize.Height,
                Top = notesLayerLayout.Y.IsPercentage ? notesLayerLayout.Y.Value * clientSize.Height : notesLayerLayout.Y.Value,
                Bottom = tapPointsLayout.Y.IsPercentage ? tapPointsLayout.Y.Value * clientSize.Height : tapPointsLayout.Y.Value,
                NoteStartXRatios = tapPoints.StartXRatios,
                NoteEndXRatios = tapPoints.EndXRatios,
                TrackCount = tapPoints.EndXRatios.Length
            };

            var commonNoteMetrics = new NoteMetrics {
                StartRadius = scalingResponder.ScaleResults.Note.Start,
                EndRadius = scalingResponder.ScaleResults.Note.End
            };

            var specialNoteMetrics = new NoteMetrics {
                StartRadius = scalingResponder.ScaleResults.SpecialNote.Start,
                EndRadius = scalingResponder.ScaleResults.SpecialNote.End
            };

            var spriteBatch = theaterDays.SpriteBatch;

            spriteBatch.BeginOnBufferedVisual();

            var layerOpacity = notesLayerConfig.Data.Opacity.Value;

            foreach (var note in notes) {
                if (!NoteAnimationHelper.IsNoteVisible(note, now, animationMetrics)) {
                    continue;
                }

                var (imageIndex, isHugeNote) = GetImageIndex(note);

                if (imageIndex < 0 && !isHugeNote) {
                    continue;
                }

                if (!isHugeNote) {
                    var thisX = AnimationCalculator.GetNoteX(note, now, commonNoteMetrics, animationMetrics);
                    var thisY = AnimationCalculator.GetNoteY(note, now, commonNoteMetrics, animationMetrics);
                    var thisRadius = AnimationCalculator.GetNoteRadius(note, now, commonNoteMetrics, animationMetrics);

                    if (notesLayerConfig.Data.Style.SyncLine) {
                        // NextSync is always after PrevSync. So draw here, below the two notes.
                        if (note.HasNextSync()) {
                            var drawSyncLine = true;
                            if (note.IsSlideMiddle() || note.NextSync.IsSlideMiddle()) {
                                drawSyncLine = notesLayerConfig.Data.Style.SlideMiddleSyncLine;
                            }
                            if (drawSyncLine) {
                                var nextX = AnimationCalculator.GetNoteX(note.NextSync, now, commonNoteMetrics, animationMetrics);
                                spriteBatch.DrawLine(thisX, thisY, nextX, thisY, _simpleSyncLinePen, _simpleSyncLinePenWidth);
                            }
                        }
                    }

                    // Then draw the note.
                    var destRect = RectHelper.RoundToRectangle(thisX - thisRadius.X / 2, thisY - thisRadius.Y / 2, thisRadius.X, thisRadius.Y);
                    spriteBatch.Draw(noteImages[0], imageIndex, destRect, layerOpacity);
                } else {
                    if (_specialNoteImage != null) {
                        var thisX = AnimationCalculator.GetSpecialNoteX(note, now, specialNoteMetrics, animationMetrics);
                        var thisY = AnimationCalculator.GetSpecialNoteY(note, now, specialNoteMetrics, animationMetrics);
                        var thisRadius = AnimationCalculator.GetSpecialNoteRadius(note, now, specialNoteMetrics, animationMetrics);
                        var destRect = RectHelper.RoundToRectangle(thisX - thisRadius.X / 2, thisY - thisRadius.Y / 2, thisRadius.X, thisRadius.Y);
                        spriteBatch.Draw(_specialNoteImage, destRect);
                    }
                }
            }

            spriteBatch.End();
        }

        protected override void OnLoadContents() {
            base.OnLoadContents();

            var theaterDays = Game.ToBaseGame();
            var debugOverlay = theaterDays.FindSingleElement<DebugOverlay>();
            var config = ConfigurationStore.Get<NotesLayerConfig>();

            var scalingResponder = theaterDays.FindSingleElement<MltdStageScalingResponder>();
            if (scalingResponder == null) {
                throw new InvalidOperationException();
            }

            if (config.Data.Images.Notes == null || config.Data.Images.Notes.Length == 0) {
                if (debugOverlay != null) {
                    debugOverlay.AddLine("WARNING: default notes image strip is not specified.");
                }
            } else {
                _noteImages = new SpriteSheet1D[config.Data.Images.Notes.Length];

                for (var i = 0; i < config.Data.Images.Notes.Length; ++i) {
                    var notesImageEntry = config.Data.Images.Notes[i];
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

                    var imageStrip = ContentHelper.LoadSpriteSheet1D(theaterDays.GraphicsDevice, notesImageEntry.File, notesImageEntry.Count, (SpriteSheetOrientation)notesImageEntry.Orientation);
                    _noteImages[i] = imageStrip;
                }
            }

            if (config.Data.Images.SpecialNote.FileName == null) {
                if (debugOverlay != null) {
                    debugOverlay.AddLine("WARNING: huge note image is not specified.");
                }
            } else if (!File.Exists(config.Data.Images.SpecialNote.FileName)) {
                if (debugOverlay != null) {
                    debugOverlay.AddLine($"WARNING: huge note image file <{config.Data.Images.SpecialNote.FileName}> is not found.");
                }
            } else {
                _specialNoteImage = ContentHelper.LoadTexture(theaterDays.GraphicsDevice, config.Data.Images.SpecialNote.FileName);
            }

            _simpleSyncLinePen = Color.White;
            _simpleSyncLinePenWidth = scalingResponder.ScaleResults.SyncLine.X;
        }

        protected override void OnUnloadContents() {
            base.OnUnloadContents();

            if (_noteImages != null) {
                foreach (var noteImage in _noteImages) {
                    noteImage?.Dispose();
                }
            }
            _noteImages = null;

            _specialNoteImage?.Dispose();
        }

        protected override void OnInitialize() {
            base.OnInitialize();

            var theaterDays = Game.ToBaseGame();

            var scoreLoader = theaterDays.FindSingleElement<ScoreLoader>();

            var config = ConfigurationStore.Get<NotesLayerConfig>();

            var traceCalculators = theaterDays.PluginManager.GetPluginsOfType<INoteAnimationCalculator>();
            if (traceCalculators.Count == 0) {
                throw new InvalidOperationException("You need at least 1 note trace calculator.");
            }

            var debugOverlay = theaterDays.FindSingleElement<DebugOverlay>();

            var calculator = traceCalculators.FirstOrDefault(calc => calc.PluginID == config.Data.Style.TracePluginID);
            if (calculator == null) {
                if (debugOverlay != null) {
                    debugOverlay.AddLine($"Missing note trace calculator with plugin ID '{config.Data.Style.TracePluginID}'. Using the first loaded calculator '{traceCalculators[0].PluginID}'.");
                }
                calculator = traceCalculators[0];
            }

            AnimationCalculator = calculator;

            GlobalSpeedScale = config.Data.Simulation.GlobalSpeed.Value;
        }

        private static (int ImageIndex, bool IsHugeNote) GetImageIndex(RuntimeNote note) {
            return GetImageIndex(note.Type, note.Size, note.FlickDirection, note.IsHoldStart(), note.IsHoldEnd(), note.IsSlideStart(), note.IsSlideEnd());
        }

        [CanBeNull]
        private Texture2D _specialNoteImage;

        [CanBeNull, ItemCanBeNull]
        private SpriteSheet1D[] _noteImages;

        private Color _simpleSyncLinePen;
        private float _simpleSyncLinePenWidth;

        private float _globalSpeedScale = 1f;

    }
}
