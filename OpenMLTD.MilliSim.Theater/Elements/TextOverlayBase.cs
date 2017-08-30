using System;
using System.Drawing;
using OpenMLTD.MilliSim.Rendering;

namespace OpenMLTD.MilliSim.Theater.Elements {
    public abstract class TextOverlayBase : Element2D {

        public event EventHandler<EventArgs> TextChanged;

        public override string Name { get; set; } = "Text Overlay Base";

        public string Text {
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

        private string _text;

    }
}
