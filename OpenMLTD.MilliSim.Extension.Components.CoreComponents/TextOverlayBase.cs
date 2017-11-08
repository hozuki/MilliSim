using System;
using System.Drawing;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Extension.Components.CoreComponents.Configuration;
using OpenMLTD.MilliSim.Graphics;
using OpenMLTD.MilliSim.Graphics.Drawing;

namespace OpenMLTD.MilliSim.Extension.Components.CoreComponents {
    public abstract class TextOverlayBase : Visual2D {

        protected TextOverlayBase([NotNull] IVisualContainer parent)
            : base(parent) {
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

        protected virtual void OnTextChanged(EventArgs e) {
            TextChanged?.Invoke(this, e);
        }

        protected virtual void OnBeforeTextRendering(RenderContext context, SizeF textSize, float lineHeight) {
        }

        [NotNull]
        protected virtual string GetFontFilePath() {
            var config = ConfigurationStore.Get<TextOverlayBaseConfig>();
            return config.Data.FontPath;
        }

        private string _text;

    }
}
