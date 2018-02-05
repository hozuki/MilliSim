using System.Reflection;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using OpenMLTD.MilliSim.Extension.Components.ScoreComponents.Configuration;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Graphics;

namespace OpenMLTD.MilliSim.Extension.Components.ScoreComponents {
    public class MltdStageScalingResponder : BaseGameComponent {

        public MltdStageScalingResponder([NotNull] BaseGame game, [NotNull] IVisualContainer parent)
            : base(game, parent) {
        }

        public ScalingConfig.ScalingConfigData ScaleResults { get; } = new ScalingConfig.ScalingConfigData();

        public Vector2 ScaledRatio { get; private set; }

        // This actually should be OnGotContext
        protected override void OnLoadContents() {
            // Handle scaling
            var s = ConfigurationStore.Get<ScalingConfig>();
            var t = ScaleResults;
            var baseScaling = s.Data.Base;
            var viewport = Game.GraphicsDevice.Viewport;
            var xRatio = viewport.Width / baseScaling.X;
            var yRatio = viewport.Height / baseScaling.Y;

            var ty = typeof(ScalingConfig.ScalingConfigData);
            var props = ty.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (var prop in props) {
                if (prop.PropertyType == typeof(Vector2)) {
                    var source = (Vector2)prop.GetValue(s.Data);
                    var scaled = Resize(source, xRatio, yRatio);
                    prop.SetValue(t, scaled);
                } else if (prop.PropertyType == typeof(ScalingConfig.SizableScaling)) {
                    var source = (ScalingConfig.SizableScaling)prop.GetValue(s.Data);
                    var scaled = Resize(source, xRatio, yRatio);
                    prop.SetValue(t, scaled);
                }
            }

            ScaledRatio = new Vector2(xRatio, yRatio);

            base.OnLoadContents();
        }

        private static Vector2 Resize(Vector2 source, float xRatio, float yRatio) {
            return new Vector2(source.X * xRatio, source.Y * yRatio);
        }

        private static ScalingConfig.SizableScaling Resize(ScalingConfig.SizableScaling source, float xRatio, float yRatio) {
            var r = new ScalingConfig.SizableScaling();
            r.Start = Resize(source.Start, xRatio, yRatio);
            r.End = Resize(source.End, xRatio, yRatio);
            return r;
        }

    }
}
