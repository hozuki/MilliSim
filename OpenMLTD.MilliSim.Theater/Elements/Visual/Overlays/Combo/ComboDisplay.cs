using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Core.Entities;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Foundation.Extensions;
using OpenMLTD.MilliSim.Graphics.Drawing;
using OpenMLTD.MilliSim.Theater.Elements.Logical;
using OpenMLTD.MilliSim.Theater.Extensions;

namespace OpenMLTD.MilliSim.Theater.Elements.Visual.Overlays.Combo {
    public class ComboDisplay : BufferedContainerElement {

        public ComboDisplay(GameBase game, [CanBeNull] [ItemNotNull] IReadOnlyList<IElement> elements)
            : base(game, elements) {
        }

        [NotNull]
        public ComboNumbers Numbers {
            get {
                if (_numbers != null) {
                    return _numbers;
                }

                var c = this.FindOrNull<ComboNumbers>();
                if (c == null) {
                    throw new InvalidOperationException();
                }
                _numbers = c;
                return c;
            }
        }

        [NotNull]
        public ComboAura Aura {
            get {
                if (_aura != null) {
                    return _aura;
                }

                var c = this.FindOrNull<ComboAura>();
                if (c == null) {
                    throw new InvalidOperationException();
                }
                _aura = c;
                return c;
            }
        }

        [NotNull]
        public ComboText Text {
            get {
                if (_text != null) {
                    return _text;
                }

                var c = this.FindOrNull<ComboText>();
                if (c == null) {
                    throw new InvalidOperationException();
                }
                _text = c;
                return c;
            }
        }

        public void PlaySpecialEndAnimation() {
            var syncTimer = Game.AsTheaterDays().FindSingleElement<SyncTimer>();
            if (syncTimer == null) {
                throw new InvalidOperationException();
            }

            _animationStartedTime = syncTimer.CurrentTime;
            _ongoingAnimation = OngoingAnimation.SpecialEnd;
        }

        protected override void OnUpdate(GameTime gameTime) {
            var theaterDays = Game.AsTheaterDays();

            var syncTimer = theaterDays.FindSingleElement<SyncTimer>();
            if (syncTimer == null) {
                throw new InvalidOperationException();
            }

            var currentTime = syncTimer.CurrentTime;
            if (currentTime < _animationStartedTime) {
                var score = theaterDays.FindSingleElement<ScoreLoader>()?.RuntimeScore;
                if (score == null) {
                    throw new InvalidOperationException();
                }

                var now = currentTime.TotalSeconds;
                var scorePrepareNote = score.Notes.Single(n => n.Type == NoteType.ScorePrepare);
                var specialNote = score.Notes.Single(n => n.Type == NoteType.Special);
                var specialEndNote = score.Notes.Single(n => n.Type == NoteType.SpecialEnd);

                if (now < scorePrepareNote.HitTime || (specialNote.HitTime < now && now < specialEndNote.HitTime)) {
                    Opacity = 0;
                } else {
                    // Automatically cancels the animation if the user steps back in UI.
                    Opacity = 1;
                }

                _ongoingAnimation = OngoingAnimation.None;
                // Fix suddent appearance.
                _animationStartedTime = currentTime;

                return;
            }

            if (_ongoingAnimation != OngoingAnimation.None) {
                var animationTime = (currentTime - _animationStartedTime).TotalSeconds;

                do {
                    var exitDo = false;

                    switch (_ongoingAnimation) {
                        case OngoingAnimation.ScorePrepare:
                            if (animationTime > _scorePrepareDuration) {
                                Opacity = 1;
                                _ongoingAnimation = OngoingAnimation.None;
                                exitDo = true;
                            }
                            break;
                        case OngoingAnimation.SpecialEnd:
                            if (animationTime > _specialEndDuration) {
                                Opacity = 1;
                                _ongoingAnimation = OngoingAnimation.None;
                                exitDo = true;
                            }
                            break;
                        case OngoingAnimation.None:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    if (exitDo) {
                        break;
                    }

                    float perc;
                    switch (_ongoingAnimation) {
                        case OngoingAnimation.ScorePrepare:
                            perc = (float)animationTime / (float)_scorePrepareDuration;
                            Opacity = perc;
                            break;
                        case OngoingAnimation.SpecialEnd:
                            perc = (float)animationTime / (float)_specialEndDuration;
                            Opacity = perc;
                            break;
                        case OngoingAnimation.None:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                } while (false);
            }

            base.OnUpdate(gameTime);
        }

        protected override void OnInitialize() {
            base.OnInitialize();

            Opacity = 0;
        }

        private ComboAura _aura;
        private ComboText _text;
        private ComboNumbers _numbers;

        private OngoingAnimation _ongoingAnimation = OngoingAnimation.None;
        private TimeSpan _animationStartedTime = TimeSpan.Zero;

        private readonly double _scorePrepareDuration = 1.5;
        private readonly double _specialEndDuration = 1.5;

        private enum OngoingAnimation {

            None = 0,
            ScorePrepare = 1,
            SpecialEnd = 2

        }

    }
}
