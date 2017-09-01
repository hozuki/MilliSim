using JetBrains.Annotations;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Graphics;
using OpenMLTD.MilliSim.Graphics.Drawing;
using OpenMLTD.MilliSim.Graphics.Drawing.Direct2D;
using OpenMLTD.MilliSim.Graphics.Extensions;

namespace OpenMLTD.MilliSim.Theater.Elements {
    public sealed class BackgroundImage : BackgroundBase {

        public BackgroundImage(GameBase game)
            : base(game) {
        }

        public void Load([NotNull] string path) {
            _filePath = path;
        }

        public void Unload() {
            _filePath = null;
        }

        protected override void OnDraw(GameTime gameTime, RenderContext context) {
            base.OnDraw(gameTime, context);

            if (_filePath != null) {
                if (_bitmap == null) {
                    OnGotContext(context);
                }
            } else {
                if (_bitmap != null) {
                    OnLostContext(context);
                }
            }

            if (_bitmap != null) {
                var location = Location;
                context.Begin2D();
                context.DrawBitmap(_bitmap, location.X, location.Y);
                context.End2D();
            }
        }

        protected override void OnGotContext(RenderContext context) {
            base.OnGotContext(context);
            if (_filePath != null) {
                _bitmap = Direct2DHelper.LoadBitmap(_filePath, context);
            }
        }

        protected override void OnLostContext(RenderContext context) {
            base.OnLostContext(context);
            _bitmap?.Dispose();
            _bitmap = null;
        }

        private string _filePath;
        private D2DBitmap _bitmap;

    }
}
