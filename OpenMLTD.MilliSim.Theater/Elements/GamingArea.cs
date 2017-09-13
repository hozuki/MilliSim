using JetBrains.Annotations;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Graphics;
using OpenMLTD.MilliSim.Graphics.Drawing;
using OpenMLTD.MilliSim.Theater.Configuration;
using System.Collections.Generic;
using System.Drawing;

namespace OpenMLTD.MilliSim.Theater.Elements {
    public class GamingArea : ContainerElement {

        public GamingArea(GameBase game, [CanBeNull] [ItemNotNull] IReadOnlyList<IElement> elements)
            : base(game, elements) {
        }

        public ScalingClass ScaleResults { get; } = new ScalingClass();

        public SizeF ScaledRatio { get; private set; } = new SizeF(1f, 1f);

        protected override void OnGotContext(RenderContext context) {
            // Handle scaling
            var s = Program.Settings.Scaling;
            var t = ScaleResults;
            var baseScaling = s.Base;
            var clientSize = context.ClientSize;
            var xRatio = clientSize.Width / baseScaling.Width;
            var yRatio = clientSize.Height / baseScaling.Height;

            t.TapBarChain = new SizeF(s.TapBarChain.Width * xRatio, s.TapBarChain.Height * yRatio);
            t.TapBarNode = new SizeF(s.TapBarNode.Width * xRatio, s.TapBarNode.Height * yRatio);
            t.TapPoint = new SizeF(s.TapPoint.Width * xRatio, s.TapPoint.Height * yRatio);
            t.Note.Start = new SizeF(s.Note.Start.Width * xRatio, s.Note.Start.Height * yRatio);
            t.Note.End = new SizeF(s.Note.End.Width * xRatio, s.Note.End.Height * yRatio);
            t.SpecialNote.Start = new SizeF(s.SpecialNote.Start.Width * xRatio, s.SpecialNote.Start.Height * yRatio);
            t.SpecialNote.End = new SizeF(s.SpecialNote.End.Width * xRatio, s.SpecialNote.End.Height * yRatio);
            t.SyncLine = new SizeF(s.SyncLine.Width * xRatio, s.SyncLine.Height * yRatio);
            t.Ribbon = new SizeF(s.Ribbon.Width * xRatio, s.Ribbon.Height * yRatio);

            ScaledRatio = new SizeF(xRatio, yRatio);

            base.OnGotContext(context);
        }

    }
}
