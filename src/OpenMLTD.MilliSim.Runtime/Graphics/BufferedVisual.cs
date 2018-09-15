using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Foundation.Extensions;
using OpenMLTD.MilliSim.Graphics.Extensions;

namespace OpenMLTD.MilliSim.Graphics {
    /// <inheritdoc cref="Visual"/>
    /// <inheritdoc cref="IBufferedVisual"/>
    /// <summary>
    /// A basic implementation for <see cref="T:OpenMLTD.MilliSim.Graphics.IBufferedVisual" />. This class must be inherited.
    /// </summary>
    public abstract class BufferedVisual : Visual, IBufferedVisual {

        /// <summary>
        /// Creates a new <see cref="BufferedVisual"/>.
        /// </summary>
        /// <param name="game">The base game.</param>
        /// <param name="parent">The parent of this <see cref="BufferedVisual"/>.</param>
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

        /// <summary>
        /// Returns a custom <see cref="RenderTarget2D"/> instance used for render buffer.
        /// This method is called during initialization.
        /// </summary>
        /// <returns>A <see cref="RenderTarget2D"/> instance.</returns>
        [NotNull]
        protected virtual RenderTarget2D CreateBufferTarget() {
            return Game.GraphicsDevice.CreateCompatibleRenderTargetFromBackBuffer();
        }

        protected override void OnInitialize() {
            base.OnInitialize();

            BufferTarget = CreateBufferTarget();
        }

        /// <summary>
        /// Called when drawing buffered contents. The drawn contents will be used in postprocessing.
        /// </summary>
        /// <param name="gameTime">Current time.</param>
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
