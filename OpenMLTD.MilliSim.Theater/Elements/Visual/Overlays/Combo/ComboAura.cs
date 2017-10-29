using System;
using System.Drawing;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Graphics;
using OpenMLTD.MilliSim.Graphics.Drawing;
using OpenMLTD.MilliSim.Graphics.Drawing.Direct2D;
using OpenMLTD.MilliSim.Graphics.Extensions;
using OpenMLTD.MilliSim.Theater.Elements.Logical;
using OpenMLTD.MilliSim.Theater.Elements.Visual.Gaming;
using OpenMLTD.MilliSim.Theater.Extensions;

namespace OpenMLTD.MilliSim.Theater.Elements.Visual.Overlays.Combo {
    public class ComboAura : BufferedVisual2D {

        public ComboAura([NotNull] IVisualContainer parent)
            : base(parent) {
        }

        public void StartAnimation() {
            var syncTimer = Game.AsTheaterDays().FindSingleElement<SyncTimer>();
            if (syncTimer == null) {
                throw new InvalidOperationException();
            }

            _animationStartedTime = syncTimer.CurrentTime;
            _isAnimationStarted = true;
        }

        protected override void OnUpdate(GameTime gameTime) {
            base.OnUpdate(gameTime);

            if (!_isAnimationStarted) {
                return;
            }

            var theaterDays = Game.AsTheaterDays();

            var syncTimer = theaterDays.FindSingleElement<SyncTimer>();
            if (syncTimer == null) {
                throw new InvalidOperationException();
            }

            var currentTime = syncTimer.CurrentTime;
            if (currentTime < _animationStartedTime) {
                // Automatically cancels the animation if the user steps back in UI.
                _isAnimationStarted = false;
                Opacity = 0;
                return;
            }

            var animationTime = (currentTime - _animationStartedTime).TotalSeconds;

            if (animationTime > _stage1Duration + _stage2Duration) {
                Opacity = 0;
                _isAnimationStarted = false;
                return;
            }

            if (animationTime > _stage1Duration) {
                Opacity = 1 - (float)(animationTime - _stage1Duration) / (float)_stage2Duration;
            } else {
                Opacity = (float)animationTime / (float)_stage1Duration;
            }
        }

        protected override void OnDrawBuffer(GameTime gameTime, RenderContext context) {
            base.OnDrawBuffer(gameTime, context);

            if (!_isAnimationStarted) {
                return;
            }

            var theaterDays = Game.AsTheaterDays();

            var gamingArea = theaterDays.FindSingleElement<GamingArea>();
            if (gamingArea == null) {
                throw new InvalidOperationException();
            }

            var scaledSize = gamingArea.ScaleResults.ComboAura;
            var location = Location;

            context.Begin2D();
            context.DrawBitmap(_auraImage, location.X, location.Y, scaledSize.Width, scaledSize.Height);
            context.End2D();
        }

        protected override void OnGotContext(RenderContext context) {
            base.OnGotContext(context);

            var settings = Program.Settings;

            _auraImage = Direct2DHelper.LoadBitmap(context, settings.Images.Combo.Aura.FileName);

            var clientSize = context.ClientSize;
            var layout = settings.UI.Combo.Aura.Layout;

            var x = layout.X.ToActualValue(clientSize.Width);
            var y = layout.Y.ToActualValue(clientSize.Height);

            Location = new Point((int)x, (int)y);
        }

        protected override void OnLostContext(RenderContext context) {
            _auraImage.Dispose();

            base.OnLostContext(context);
        }

        protected override void OnInitialize() {
            base.OnInitialize();

            Opacity = 0;
        }

        internal static readonly int[] ComboCountTriggers = {
            10, 20, 50, 100, 200, 500, 1000, 2000, 5000
        };

        private D2DBitmap _auraImage;

        private readonly double _stage1Duration = 0.2;
        private readonly double _stage2Duration = 2;

        private bool _isAnimationStarted;
        private TimeSpan _animationStartedTime;

    }
}
