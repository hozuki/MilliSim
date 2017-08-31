using System.Drawing;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Rendering;

namespace OpenMLTD.MilliSim.Theater.Elements {
    public class HelpOverlay : OutlinedTextOverlay {

        public HelpOverlay(GameBase game)
            : base(game) {
        }

        public override float FontSize { get; set; } = 30;

        public override float StrokeWidth { get; set; } = 5;

        protected override void OnBeforeTextRendering(RenderContext context, SizeF textSize, float lineHeight) {
            base.OnBeforeTextRendering(context, textSize, lineHeight);
            var clientSize = context.ClientSize;
            var left = (clientSize.Width - textSize.Width) / 2;
            var top = clientSize.Height * 0.75f - textSize.Height / 2;
            Location = new Point((int)left, (int)top);
        }

    }
}
