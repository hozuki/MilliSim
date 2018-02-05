using System;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using OpenMLTD.MilliSim.Contributed.Scores;
using OpenMLTD.MilliSim.Extension.Components.CoreComponents;
using OpenMLTD.MilliSim.Extension.Components.ScoreComponents.Overlays.Combo;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Foundation.Extensions;
using OpenMLTD.MilliSim.Graphics;

namespace OpenMLTD.MilliSim.Extension.Components.ScoreComponents.Overlays {
    public class ComboDisplay : BufferedVisualContainer {

        public ComboDisplay([NotNull] BaseGame game, [NotNull] IVisualContainer parent)
            : base(game, parent) {
        }

        [NotNull]
        public ComboNumbers Numbers {
            get {
                if (_numbers != null) {
                    return _numbers;
                }

                var c = this.FindSingleOrNull<ComboNumbers>();
                _numbers = c ?? throw new InvalidOperationException();
                return c;
            }
        }

        [NotNull]
        public ComboAura Aura {
            get {
                if (_aura != null) {
                    return _aura;
                }

                var c = this.FindSingleOrNull<ComboAura>();
                _aura = c ?? throw new InvalidOperationException();
                return c;
            }
        }

        [NotNull]
        public ComboText Text {
            get {
                if (_text != null) {
                    return _text;
                }

                var c = this.FindSingleOrNull<ComboText>();
                _text = c ?? throw new InvalidOperationException();
                return c;
            }
        }

        public void PlaySpecialEndAnimation() {
            var syncTimer = Game.ToBaseGame().FindSingleElement<SyncTimer>();
            if (syncTimer == null) {
                throw new InvalidOperationException();
            }

            _animationStartedTime = syncTimer.CurrentTime;
            _ongoingAnimation = OngoingAnimation.SpecialEnd;
        }

        protected override void OnSelfUpdate(GameTime gameTime) {
            var theaterDays = Game.ToBaseGame();

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

            base.OnSelfUpdate(gameTime);
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
