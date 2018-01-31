using System;
using System.Drawing;
using JetBrains.Annotations;
using MonoGame.Extended.Text;
using OpenMLTD.MilliSim.Extension.Components.CoreComponents.Configuration;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Graphics;

namespace OpenMLTD.MilliSim.Extension.Components.CoreComponents {
    public abstract class TextOverlayBase : Visual {

        protected TextOverlayBase([NotNull] BaseGame game, [NotNull] IVisualContainer parent)
            : base(game, parent) {
            if (_referenceCount == 0) {
                _fontManager = new FontManager();
            }

            ++_referenceCount;
        }

        public event EventHandler<EventArgs> TextChanged;

        public Point Location { get; set; }

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

        protected virtual void OnTextChanged(EventArgs e) {
            TextChanged?.Invoke(this, e);
        }

        [NotNull]
        protected virtual string GetFontFilePath() {
            var config = ConfigurationStore.Get<TextOverlayBaseConfig>();
            return config.Data.FontPath;
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                --_referenceCount;

                if (_referenceCount <= 0) {
                    _referenceCount = 0;

                    _fontManager.Dispose();

                    _fontManager = null;
                }
            }

            base.Dispose(disposing);
        }

        protected FontManager FontManager => _fontManager;

        private string _text = string.Empty;

        private static FontManager _fontManager;
        private static int _referenceCount;

    }
}
