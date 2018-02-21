using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Foundation.Extensions;
using OpenMLTD.MilliSim.Graphics.Extensions;

namespace OpenMLTD.MilliSim.Graphics {
    public abstract class BufferedVisual : Visual, IBufferedVisual {

        protected BufferedVisual([NotNull] BaseGame game, [CanBeNull] IVisualContainer parent)
            : base(game, parent) {
        }

        public RenderTarget2D BufferTarget { get; private set; }

        public float Opacity {
            get => _opacity;
            set => _opacity = MathHelper.Clamp(value, 0, 1);
        }

        public Matrix? Transform { get; set; }

        public Vector2 Location { get; set; }

        protected virtual RenderTarget2D CreateBufferTarget() {
            return Game.GraphicsDevice.CreateCompatibleRenderTargetFromBackBuffer();
        }

        protected override void OnInitialize() {
            base.OnInitialize();

            BufferTarget = CreateBufferTarget();
        }

        protected virtual void OnDrawBuffer(GameTime gameTime) {
        }

        protected sealed override void OnDraw(GameTime gameTime) {
            var game = Game.ToBaseGame();
            var graphicsDevice = game.GraphicsDevice;

            using (graphicsDevice.SwitchTo(BufferTarget)) {
                base.OnDraw(gameTime);

                OnDrawBuffer(gameTime);
            }

            game.SpriteBatch.DrawBufferedVisual(this);
        }

        protected override void Dispose(bool disposing) {
            BufferTarget?.Dispose();
            BufferTarget = null;

            base.Dispose(disposing);
        }

        private float _opacity = 1f;

    }
}
