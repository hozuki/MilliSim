using System;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Overlay;
using OpenMLTD.MilliSim.Extension.Components.CoreComponents.Configuration;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Foundation.Extensions;
using OpenMLTD.MilliSim.Graphics;

namespace OpenMLTD.MilliSim.Extension.Components.CoreComponents {
    public abstract class TextOverlayBase : OverlayBase {

        protected TextOverlayBase([NotNull] BaseGame game, [NotNull] IVisualContainer parent)
            : base(game, parent) {
        }

        public event EventHandler<EventArgs> TextChanged;

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

        public virtual Color FillColor { get; set; } = Color.White;

        public virtual float FontSize { get; set; } = 10;

        protected override void OnLoadContents() {
            base.OnLoadContents();

            var theaterDays = Game.ToBaseGame();
            var fontPath = System.IO.Path.GetFullPath(GetFontFilePath());

            _font = theaterDays.FontManager.LoadFont(fontPath);
            _font.Size = FontSize;
        }

        protected override void OnDraw(GameTime gameTime) {
            if (_shouldRedrawText) {
                var measuredSize = MeasureText(gameTime);

                Graphics.Clear(Color.Transparent);
                OnDrawText(gameTime, measuredSize);

                _shouldRedrawText = false;
            }

            base.OnDraw(gameTime);
        }

        protected abstract void OnDrawText([NotNull] GameTime gameTime, [CanBeNull] Vector2? measuredSize);

        protected virtual void OnTextChanged(EventArgs e) {
            TextChanged?.Invoke(this, e);
            _shouldRedrawText = true;
        }

        [NotNull]
        protected virtual string GetFontFilePath() {
            var config = ConfigurationStore.Get<TextOverlayBaseConfig>();
            return config.Data.FontPath;
        }

        protected virtual Vector2? MeasureText([NotNull] GameTime gameTime) {
            return null;
        }

        protected Font Font => _font;

        private string _text = string.Empty;
        // Graphics retains drawn contents, so we have to lazily redraw.
        private bool _shouldRedrawText;

        private Font _font;


    }
}
