using System.Drawing;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.GameAbstraction.Extensions;
using OpenMLTD.MilliSim.Graphics;
using OpenMLTD.MilliSim.Graphics.Drawing;

namespace OpenMLTD.MilliSim.Extension.Components.CoreComponents {
    public class SyncTimerOverlay : TextOverlay {

        public SyncTimerOverlay([NotNull] IVisualContainer parent)
            : base(parent) {
        }

        protected override void OnUpdate(GameTime gameTime) {
            base.OnUpdate(gameTime);

            var syncTimer = Game.AsTheaterDays().FindSingleElement<SyncTimer>();
            if (syncTimer != null) {
                Text = syncTimer.CurrentTime.ToString(@"hh\:mm\:ss\.fff");
            }
        }

        protected override void OnGotContext(RenderContext context) {
            base.OnGotContext(context);

            var fps = Game.AsTheaterDays().FindSingleElement<FpsOverlay>();
            var fpsHeight = fps != null ? (int)DirectWriteHelper.PointToDip(fps.FontSize) : 0;
            var clientSize = context.ClientSize;
            Location = new Point(clientSize.Width - 80, fpsHeight);
        }

    }
}
