using System.Drawing;
using System.Reflection;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Extension.Components.ScoreComponents.Configuration;
using OpenMLTD.MilliSim.Graphics;

namespace OpenMLTD.MilliSim.Extension.Components.ScoreComponents {
    public class MltdStageScalingResponder : Visual {

        public MltdStageScalingResponder([NotNull] IVisualContainer parent)
            : base(parent) {
        }

        public ScalingConfig.ScalingConfigData ScaleResults { get; } = new ScalingConfig.ScalingConfigData();

        public SizeF ScaledRatio { get; private set; }

        protected override void OnGotContext(RenderContext context) {
            // Handle scaling
            var s = ConfigurationStore.Get<ScalingConfig>();
            var t = ScaleResults;
            var baseScaling = s.Data.Base;
            var clientSize = context.ClientSize;
            var xRatio = clientSize.Width / baseScaling.Width;
            var yRatio = clientSize.Height / baseScaling.Height;

            var ty = typeof(ScalingConfig.ScalingConfigData);
            var props = ty.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (var prop in props) {
                if (prop.PropertyType == typeof(SizeF)) {
                    var source = (SizeF)prop.GetValue(s.Data);
                    var scaled = Resize(source, xRatio, yRatio);
                    prop.SetValue(t, scaled);
                } else if (prop.PropertyType == typeof(ScalingConfig.SizableScaling)) {
                    var source = (ScalingConfig.SizableScaling)prop.GetValue(s.Data);
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

        private static ScalingConfig.SizableScaling Resize(ScalingConfig.SizableScaling source, float xRatio, float yRatio) {
            var r = new ScalingConfig.SizableScaling();
            r.Start = Resize(source.Start, xRatio, yRatio);
            r.End = Resize(source.End, xRatio, yRatio);
            return r;
        }

    }
}
