using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Core.Entities.Runtime;
using OpenMLTD.MilliSim.Core.Entities.Runtime.Extensions;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Theater.Extensions;
using OpenMLTD.MilliSim.Theater.Intenal;

namespace OpenMLTD.MilliSim.Theater.Elements {
    public class NoteSfxPlayer : Element {

        public NoteSfxPlayer(GameBase game)
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

            var sfxPaths = Program.Settings.Sfx;
            var player = theaterDays.AudioManager.Sfx;
            var now = syncTimer.CurrentTime.TotalSeconds;

            var noteMetrics = new NoteMetrics {
                GlobalSpeedScale = notesLayer.GlobalSpeedScale
            };

            var states = _noteStates;

            foreach (var note in _notes) {
                var oldState = states[note];
                var newState = NoteAnimationHelper.GetOnStageStatusOf(note, now, noteMetrics);

                if (oldState == newState) {
                    continue;
                }

                switch (note.Type) {
                    case RuntimeNoteType.Tap:
                        if (newState == OnStageStatus.Passed) {
                            player.Play(sfxPaths.Tap.Perfect);
                        }
                        break;
                    case RuntimeNoteType.Flick:
                        if (newState == OnStageStatus.Passed) {
                            player.Play(sfxPaths.Flick.Perfect);
                        }
                        break;
                    case RuntimeNoteType.Hold:
                        if (note.IsHoldStart()) {
                            if (note.FlickDirection != FlickDirection.None) {
                                if (newState == OnStageStatus.Passed) {
                                    player.Play(sfxPaths.Flick.Perfect);
                                }
                            } else {
                                if (newState == OnStageStatus.Passed) {
                                    player.Play(sfxPaths.Hold.Perfect);
                                    player.PlayLooped(sfxPaths.HoldHold, note);
                                }
                            }
                        } else if (note.IsHoldEnd()) {
                            if (note.FlickDirection != FlickDirection.None) {
                                if (newState == OnStageStatus.Passed) {
                                    player.Play(sfxPaths.Flick.Perfect);
                                }
                            } else {
                                if (newState == OnStageStatus.Passed) {
                                    player.Play(sfxPaths.HoldEnd.Perfect);
                                }
                            }

                            if (newState == OnStageStatus.Passed) {
                                player.StopLooped(FindFirstHold(note));
                            }
                        }
                        break;
                    case RuntimeNoteType.Slide:
                        if (note.IsSlideStart()) {
                            if (note.FlickDirection != FlickDirection.None) {
                                if (newState == OnStageStatus.Passed) {
                                    player.Play(sfxPaths.Flick.Perfect);
                                }
                            } else {
                                if (newState == OnStageStatus.Passed) {
                                    player.Play(sfxPaths.Slide.Perfect);
                                    player.PlayLooped(sfxPaths.SlideHold, note);
                                }
                            }
                        } else if (note.IsSlideEnd()) {
                            if (note.FlickDirection != FlickDirection.None) {
                                if (newState == OnStageStatus.Passed) {
                                    player.Play(sfxPaths.Flick.Perfect);
                                }
                            } else {
                                if (newState == OnStageStatus.Passed) {
                                    player.Play(sfxPaths.SlideEnd.Perfect);
                                }
                            }

                            if (newState == OnStageStatus.Passed) {
                                player.StopLooped(FindFirstSlide(note));
                            }
                        }
                        break;
                    case RuntimeNoteType.Special:
                        if (newState == OnStageStatus.Passed) {
                            player.Play(sfxPaths.Special.Perfect);
                        }
                        break;
                    case RuntimeNoteType.SpecialEnd:
                        if (newState == OnStageStatus.Passed) {
                            player.Play(sfxPaths.SpecialEnd);
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
        private static readonly Dictionary<RuntimeNote, OnStageStatus> _noteStates = new Dictionary<RuntimeNote, OnStageStatus>();

    }
}
