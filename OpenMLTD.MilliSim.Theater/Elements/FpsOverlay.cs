using System;
using System.Drawing;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Graphics;

namespace OpenMLTD.MilliSim.Theater.Elements {
    public class FpsOverlay : TextOverlay {

        public FpsOverlay(GameBase game)
            : base(game) {
        }

        public override string Name { get; set; } = "FPS Overlay";

        public override float FontSize { get; set; } = 15;

        protected override void OnBeforeTextRendering(RenderContext context, SizeF textSize, float lineHeight) {
            base.OnBeforeTextRendering(context, textSize, lineHeight);
            var left = context.ClientSize.Width - textSize.Width;
            const int top = 0;
            Location = new Point((int)left, top);
        }

        protected override void OnUpdate(GameTime gameTime) {
            base.OnUpdate(gameTime);

            var now = DateTime.UtcNow;
            if (now - _lastUpdatedTime >= _updateInterval) {
                var timeDiff = gameTime.Total - _lastTotalTime;

                float fps;
                var seconds = (float)timeDiff.TotalSeconds;
                if (seconds.Equals(0f)) {
                    fps = 0;
                } else {
                    fps = _frameCounter / seconds;
                }

                Text = $"FPS: {fps:0.##}";

                _lastTotalTime = gameTime.Total;
                _lastUpdatedTime = now;
                _frameCounter = 0;
            }

            ++_frameCounter;
        }

        private readonly TimeSpan _updateInterval = TimeSpan.FromSeconds(1);

        private TimeSpan _lastTotalTime = TimeSpan.Zero;

        private DateTime _lastUpdatedTime = DateTime.MinValue;
        private int _frameCounter;

    }
}
