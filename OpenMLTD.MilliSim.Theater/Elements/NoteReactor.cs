using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Core.Entities;
using OpenMLTD.MilliSim.Core.Entities.Extensions;
using OpenMLTD.MilliSim.Core.Entities.Runtime;
using OpenMLTD.MilliSim.Core.Entities.Source;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Theater.Animation;
using OpenMLTD.MilliSim.Theater.Extensions;

namespace OpenMLTD.MilliSim.Theater.Elements {
    /// <summary>
    /// Reacting to note events.
    /// </summary>
    public class NoteReactor : Element {

        public NoteReactor(GameBase game)
            : base(game) {
        }

        protected override void OnUpdate(GameTime gameTime) {
            base.OnUpdate(gameTime);

            if (_notes == null) {
                return;
            }

            var theaterDays = Game.AsTheaterDays();

            var notesLayer = theaterDays.FindSingleElement<NotesLayer>();
            if (notesLayer == null) {
                throw new InvalidOperationException();
            }

            var syncTimer = theaterDays.FindSingleElement<SyncTimer>();
            if (syncTimer == null) {
                throw new InvalidOperationException();
            }

            var audioFormats = Program.PluginManager.AudioFormats;

            var sfxPaths = Program.Settings.Sfx;
            var player = theaterDays.AudioManager.Sfx;
            var now = syncTimer.CurrentTime.TotalSeconds;
            var globalSpeedScale = notesLayer.GlobalSpeedScale;

            var states = _noteStates;

            foreach (var note in _notes) {
                var oldState = states[note];
                var newState = NoteAnimationHelper.GetOnStageStatusOf(note, now, globalSpeedScale);

                if (oldState == newState) {
                    continue;
                }

                switch (note.Type) {
                    case NoteType.Tap:
                        if (newState == OnStageStatus.Passed) {
                            player.Play(sfxPaths.Tap.Perfect, audioFormats);
                        }
                        break;
                    case NoteType.Flick:
                        if (newState == OnStageStatus.Passed) {
                            player.Play(sfxPaths.Flick.Perfect, audioFormats);
                        }
                        break;
                    case NoteType.Hold:
                        if (note.IsHoldStart()) {
                            if (note.FlickDirection != FlickDirection.None) {
                                if (newState == OnStageStatus.Passed) {
                                    player.Play(sfxPaths.Flick.Perfect, audioFormats);
                                }
                            } else {
                                if (newState == OnStageStatus.Passed) {
                                    player.Play(sfxPaths.Hold.Perfect, audioFormats);
                                    player.PlayLooped(sfxPaths.HoldHold, audioFormats, note);
                                }
                            }
                        } else if (note.IsHoldEnd()) {
                            if (note.FlickDirection != FlickDirection.None) {
                                if (newState == OnStageStatus.Passed) {
                                    player.Play(sfxPaths.Flick.Perfect, audioFormats);
                                }
                            } else {
                                if (newState == OnStageStatus.Passed) {
                                    player.Play(sfxPaths.HoldEnd.Perfect, audioFormats);
                                }
                            }

                            if (newState == OnStageStatus.Passed) {
                                player.StopLooped(FindFirstHold(note));
                            }
                        }
                        break;
                    case NoteType.Slide:
                        if (note.IsSlideStart()) {
                            if (note.FlickDirection != FlickDirection.None) {
                                if (newState == OnStageStatus.Passed) {
                                    player.Play(sfxPaths.Flick.Perfect, audioFormats);
                                }
                            } else {
                                if (newState == OnStageStatus.Passed) {
                                    player.Play(sfxPaths.Slide.Perfect, audioFormats);
                                    player.PlayLooped(sfxPaths.SlideHold, audioFormats, note);
                                }
                            }
                        } else if (note.IsSlideEnd()) {
                            if (note.FlickDirection != FlickDirection.None) {
                                if (newState == OnStageStatus.Passed) {
                                    player.Play(sfxPaths.Flick.Perfect, audioFormats);
                                }
                            } else {
                                if (newState == OnStageStatus.Passed) {
                                    player.Play(sfxPaths.SlideEnd.Perfect, audioFormats);
                                }
                            }

                            if (newState == OnStageStatus.Passed) {
                                player.StopLooped(FindFirstSlide(note));
                            }
                        }
                        break;
                    case NoteType.Special:
                        if (newState == OnStageStatus.Passed) {
                            player.Play(sfxPaths.Special.Perfect, audioFormats);
                            var shouts = sfxPaths.Shouts;
                            if (shouts != null && shouts.Length > 0) {
                                var shoutIndex = MathHelper.Random.Next(shouts.Length);
                                player.Play(shouts[shoutIndex], audioFormats);
                            }
                            player.PlayLooped(sfxPaths.SpecialHold, audioFormats, note);
                        }
                        break;
                    case NoteType.SpecialEnd:
                        if (newState == OnStageStatus.Passed) {
                            player.Play(sfxPaths.SpecialEnd, audioFormats);
                            var shouts = sfxPaths.Shouts;
                            if (shouts != null && shouts.Length > 0) {
                                var shoutIndex = MathHelper.Random.Next(shouts.Length);
                                player.Play(shouts[shoutIndex], audioFormats);
                            }

                            RuntimeNote specialStart = null;
                            try {
                                specialStart = _notes.SingleOrDefault(n => n.Type == NoteType.Special);
                            } catch (InvalidOperationException) {
                                // Multiple Special Start notes.
                            }
                            Debug.Assert(specialStart != null, "Wrong score format: there must be only exactly one special note and one special end note, if either of them exists.");
                            player.StopLooped(specialStart);

                            var tapPoints = theaterDays.FindSingleElement<TapPoints>();
                            if (tapPoints != null) {
                                // The animation length must keep the same as the SpecialEnd note's delay.
                                // See Unity3DScoreCompiler.CreateSpecial() for more information.
                                tapPoints.FadeIn(TimeSpan.FromSeconds(1.5));
                            }
                        }
                        break;
                    case NoteType.SpecialPrepare:
                        if (newState == OnStageStatus.Passed) {
                            var tapPoints = theaterDays.FindSingleElement<TapPoints>();
                            if (tapPoints != null) {
                                // This shall trigger tap points' "transform" animation.
                                // See Unity3DScoreCompiler.CreateSpecial() for more information.
                                // TODO: implement "transform" animation.
                                //tapPoints.PlayTransformAnimation(TimeSpan.FromSeconds(0.8));
                                tapPoints.Opacity = 0;
                            }
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                states[note] = newState;
            }
        }

        protected override void OnInitialize() {
            base.OnInitialize();

            var theaterDays = Game.AsTheaterDays();
            var scoreLoader = theaterDays.FindSingleElement<ScoreLoader>();

            var score = scoreLoader?.RuntimeScore;
            if (score != null) {
                foreach (var note in score.Notes) {
                    _noteStates.Add(note, OnStageStatus.Incoming);
                }
                _notes = score.Notes;
            }
        }

        private static RuntimeNote FindFirstHold(RuntimeNote note) {
            var firstHold = note;
            do {
                firstHold = firstHold.PrevHold;
            } while (firstHold.PrevHold != null);
            return firstHold;
        }

        private static RuntimeNote FindFirstSlide(RuntimeNote note) {
            var firstSlide = note;
            do {
                firstSlide = firstSlide.PrevSlide;
            } while (firstSlide.PrevSlide != null);
            return firstSlide;
        }

        [CanBeNull]
        private IReadOnlyList<RuntimeNote> _notes;
        private readonly Dictionary<RuntimeNote, OnStageStatus> _noteStates = new Dictionary<RuntimeNote, OnStageStatus>();

    }
}
