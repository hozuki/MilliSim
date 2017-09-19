using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Graphics;
using OpenMLTD.MilliSim.Graphics.Drawing;
using OpenMLTD.MilliSim.Theater.Configuration;

namespace OpenMLTD.MilliSim.Theater.Elements.Visual.Gaming {
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

            var ty = typeof(ScalingClass);
            var props = ty.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (var prop in props) {
                if (prop.PropertyType == typeof(SizeF)) {
                    var source = (SizeF)prop.GetValue(s);
                    var scaled = Resize(source, xRatio, yRatio);
                    prop.SetValue(t, scaled);
                } else if (prop.PropertyType == typeof(ScalingClass.SizableScaling)) {
                    var source = (ScalingClass.SizableScaling)prop.GetValue(s);
                    var scaled = Resize(source, xRatio, yRatio);
                    prop.SetValue(t, scaled);
                }
            }

            ScaledRatio = new SizeF(xRatio, yRatio);

            base.OnGotContext(context);
        }

        private static SizeF Resize(SizeF source, float xRatio, float yRatio) {
            return new SizeF(source.Width * xRatio, source.Height * yRatio);
        }

        private static ScalingClass.SizableScaling Resize(ScalingClass.SizableScaling source, float xRatio, float yRatio) {
            var r = new ScalingClass.SizableScaling();
            r.Start = Resize(source.Start, xRatio, yRatio);
            r.End = Resize(source.End, xRatio, yRatio);
            return r;
        }

    }
}
