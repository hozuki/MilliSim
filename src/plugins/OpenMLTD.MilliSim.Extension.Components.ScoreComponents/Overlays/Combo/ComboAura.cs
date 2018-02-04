using System;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpenMLTD.MilliSim.Extension.Components.CoreComponents;
using OpenMLTD.MilliSim.Extension.Components.ScoreComponents.Configuration;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Foundation.Extensions;
using OpenMLTD.MilliSim.Graphics;
using OpenMLTD.MilliSim.Graphics.Extensions;

namespace OpenMLTD.MilliSim.Extension.Components.ScoreComponents.Overlays.Combo {
    public class ComboAura : Visual, IVisual2D, ISupportsOpacity {

        public ComboAura([NotNull] BaseGame game, [NotNull] IVisualContainer parent)
            : base(game, parent) {
        }

        public Vector2 Location { get; set; }

        public float Opacity {
            get => _opacity;
            set => _opacity = MathHelper.Clamp(value, 0, 1);
        }

        public void StartAnimation() {
            var syncTimer = Game.ToBaseGame().FindSingleElement<SyncTimer>();
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

            var theaterDays = Game.ToBaseGame();

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

            var animationTime = (float)(currentTime - _animationStartedTime).TotalSeconds;

            if (animationTime > Stage1Duration + Stage2Duration) {
                Opacity = 0;
                _isAnimationStarted = false;
                return;
            }

            if (animationTime > Stage1Duration) {
                Opacity = 1 - (animationTime - Stage1Duration) / Stage2Duration;
            } else {
                Opacity = animationTime / Stage1Duration;
            }
        }

        protected override void OnDraw(GameTime gameTime) {
            base.OnDraw(gameTime);

            if (!_isAnimationStarted) {
                return;
            }

            var theaterDays = Game.ToBaseGame();

            var scalingResponder = theaterDays.FindSingleElement<MltdStageScalingResponder>();
            if (scalingResponder == null) {
                throw new InvalidOperationException();
            }

            var scaledSize = scalingResponder.ScaleResults.ComboAura;
            var location = Location;
            var spriteBatch = theaterDays.SpriteBatch;
            var destRect = RectHelper.RoundToRectangle(location.X, location.Y, scaledSize.X, scaledSize.Y);

            spriteBatch.BeginOnBufferedVisual();
            // TODO: A dirty hack, not inheriting from BufferedVisual, but manually draw the texture using Opacity.
            // If this draw call is handled inside a BufferedVisual (whithout the opacity param, i.e. always Color.White),
            // this visual disappears...
            spriteBatch.Draw(_auraImage, destRect, Opacity);
            spriteBatch.End();
        }

        protected override void OnLoadContents() {
            base.OnLoadContents();

            var game = Game.ToBaseGame();
            var graphics = game.GraphicsDevice;
            var config = ConfigurationStore.Get<ComboAuraConfig>();

            _auraImage = ContentHelper.LoadTexture(graphics, config.Data.Image.FileName);

            var clientSize = graphics.Viewport;
            var layout = config.Data.Layout;

            var x = layout.X.ToActualValue(clientSize.Width);
            var y = layout.Y.ToActualValue(clientSize.Height);

            Location = new Vector2(x, y);
        }

        protected override void OnUnloadContents() {
            _auraImage.Dispose();

            base.OnUnloadContents();
        }

        protected override void OnInitialize() {
            base.OnInitialize();

            Opacity = 1;
        }

        internal static readonly int[] ComboCountTriggers = {
            10, 20, 50, 100, 200, 500, 1000, 2000, 5000
        };

        private Texture2D _auraImage;

        private static readonly float Stage1Duration = 0.2f;
        private static readonly float Stage2Duration = 2f;

        private float _opacity;
        private bool _isAnimationStarted;
        private TimeSpan _animationStartedTime;

    }
}
