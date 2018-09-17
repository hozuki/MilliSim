using System;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Overlay;
using OpenMLTD.MilliSim.Extension.Components.CoreComponents.Configuration;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Foundation.Extensions;
using OpenMLTD.MilliSim.Graphics;

namespace OpenMLTD.MilliSim.Extension.Components.CoreComponents {
    /// <inheritdoc />
    /// <summary>
    /// Base class for text overlays. This class must be inherited.
    /// </summary>
    public abstract class TextOverlayBase : OverlayBase {

        /// <inheritdoc />
        /// <summary>
        /// Creates a new <see cref="TextOverlayBase"/>.
        /// </summary>
        /// <param name="game">The base game.</param>
        /// <param name="parent">The parent of the <see cref="TextOverlayBase"/>.</param>
        protected TextOverlayBase([NotNull] BaseGame game, [NotNull] IVisualContainer parent)
            : base(game, parent) {
        }

        /// <summary>
        /// Triggered when <see cref="Text"/> is changed.
        /// </summary>
        public event EventHandler<EventArgs> TextChanged;

        /// <summary>
        /// Gets or sets the text of this <see cref="TextOverlayBase"/>.
        /// </summary>
        public virtual string Text {
            get => _text;
            set {
                var b = _text != value;
                if (b) {
                    _text = value;
                    OnTextChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the text color.
        /// </summary>
        public virtual Color FillColor { get; set; } = Color.White;

        /// <summary>
        /// Gets or sets the size of the font, in points.
        /// </summary>
        public virtual float FontSize { get; set; } = 10;

        protected override void OnLoadContents() {
            base.OnLoadContents();

            var theaterDays = Game.ToBaseGame();
            var fontPath = System.IO.Path.GetFullPath(GetFontFilePath());

            _font = theaterDays.FontManager.LoadFont(fontPath);
            _font.Size = FontSize;
        }

        protected override void OnDraw(GameTime gameTime) {
            var graphics = Graphics;

            if (graphics != null) {
                if (_shouldRedrawText) {
                    var measuredSize = MeasureText(gameTime);

                    graphics.Clear(Color.Transparent);
                    OnDrawText(gameTime, measuredSize);

                    _shouldRedrawText = false;
                }
            }

            base.OnDraw(gameTime);
        }

        /// <summary>
        /// Called when the text is drawn. When overridden in subclasses, use this method to render the text
        /// on current render target. This method must be overridden.
        /// </summary>
        /// <param name="gameTime">Current time.</param>
        /// <param name="measuredSize">Measured text size. Can be <see langword="null"/> if there is an error or the renderer does not care.</param>
        protected abstract void OnDrawText([NotNull] GameTime gameTime, [CanBeNull] Vector2? measuredSize);

        /// <summary>
        /// Called when the text of this element is changed.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected virtual void OnTextChanged([NotNull] EventArgs e) {
            TextChanged?.Invoke(this, e);
            _shouldRedrawText = true;
        }

        /// <summary>
        /// When overridden in subclasses, this method returns the path to the font file this <see cref="TextOverlayBase"/> uses.
        /// </summary>
        /// <returns>The path to the font file.</returns>
        [NotNull]
        protected virtual string GetFontFilePath() {
            var config = ConfigurationStore.Get<TextOverlayBaseConfig>();
            return config.Data.FontPath;
        }

        /// <summary>
        /// When overridden in subclasses, measures a piece of text under current font settings, in pixels.
        /// Returning <see langword="null"/> means there is an error or there is no need for measuring.
        /// </summary>
        /// <param name="gameTime">Current time.</param>
        /// <returns>Measured text dimensions, in pixels.</returns>
        [CanBeNull]
        protected virtual Vector2? MeasureText([NotNull] GameTime gameTime) {
            return null;
        }

        /// <summary>
        /// Gets current font object.
        /// </summary>
        protected Font Font => _font;

        private string _text = string.Empty;
        // Graphics retains drawn contents, so we have to lazily redraw.
        private bool _shouldRedrawText;

        private Font _font;


    }
}
