using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Foundation.Extensions;
using OpenMLTD.MilliSim.Graphics;

namespace OpenMLTD.MilliSim.Extension.Components.CoreComponents {
    /// <inheritdoc cref="Visual"/>
    /// <inheritdoc cref="IVisual2D"/>
    /// <inheritdoc cref="ISupportsOpacity"/>
    /// <summary>
    /// The base class for all overlays. Overlays are visual elements which target rendering on the 2D screen, using Direct2D-like APIs.
    /// </summary>
    public abstract class OverlayBase : Visual, IVisual2D, ISupportsOpacity {

        /// <inheritdoc />
        /// <summary>
        /// Creates a new <see cref="OverlayBase"/>.
        /// </summary>
        /// <param name="game">The base game.</param>
        /// <param name="parent">The parent of the <see cref="OverlayBase"/>.</param>
        protected OverlayBase([NotNull] BaseGame game, [CanBeNull] IVisualContainer parent)
            : base(game, parent) {
        }

        public float Opacity {
            get => _opacity;
            set => _opacity = MathHelper.Clamp(value, 0, 1);
        }

        public Vector2 Location { get; set; }

        protected override void OnUpdate(GameTime gameTime) {
            base.OnUpdate(gameTime);

            _graphics.UpdateBackBuffer();
        }

        protected override void OnDraw(GameTime gameTime) {
            base.OnDraw(gameTime);

            var spriteBatch = Game.ToBaseGame().SpriteBatch;
            spriteBatch.Begin(blendState: BlendState.AlphaBlend);
            spriteBatch.Draw(_graphics.BackBuffer, Vector2.Zero, Color.White * Opacity);
            spriteBatch.End();
        }

        protected override void OnInitialize() {
            base.OnInitialize();

            _graphics = new MonoGame.Extended.Overlay.Graphics(Game.GraphicsDevice);
        }

        protected override void Dispose(bool disposing) {
            _graphics?.Dispose();
            _graphics = null;

            base.Dispose(disposing);
        }

        /// <summary>
        /// Gets the <see cref="MonoGame.Extended.Overlay.Graphics"/> object used for drawing 2D contents.
        /// This property is initialized during <see cref="OnInitialize"/>.
        /// </summary>
        [CanBeNull]
        protected MonoGame.Extended.Overlay.Graphics Graphics => _graphics;

        private float _opacity = 1f;
        private MonoGame.Extended.Overlay.Graphics _graphics;

    }
}
