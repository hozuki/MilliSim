using System;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Foundation.Extensions;
using OpenMLTD.MilliSim.Graphics;

namespace OpenMLTD.MilliSim.Extension.Components.CoreComponents {
    public class FpsOverlay : TextOverlay {

        public FpsOverlay([NotNull] BaseGame game, [NotNull] IVisualContainer parent)
            : base(game, parent) {
        }

        public override float FontSize { get; set; } = 15;

        protected override void OnInitialize() {
            base.OnInitialize();

            _lastUpdatedTime = DateTime.UtcNow;
        }

        protected override void OnUpdate(GameTime gameTime) {
            base.OnUpdate(gameTime);

            _fpsCounter.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

            var now = DateTime.UtcNow;

            if (now - _lastUpdatedTime >= _updateInterval) {
                var fps = _fpsCounter.Average;

                Text = "FPS: " + fps.ToString("0.##");
            }
        }

        protected override Vector2? MeasureText(GameTime gameTime) {
            var viewport = Game.ToBaseGame().GraphicsDevice.Viewport;
            var textSize = Graphics.MeasureString(Font, Text);
            var left = viewport.Width - textSize.X - 4;
            const int top = 0;

            Location = new Vector2(left, top);

            return textSize;
        }

        private readonly TimeSpan _updateInterval = TimeSpan.FromSeconds(1);

        private readonly FpsCounter _fpsCounter = new FpsCounter();

        private DateTime _lastUpdatedTime;

    }
}
