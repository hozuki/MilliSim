using System.Drawing;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Graphics;
using OpenMLTD.MilliSim.Graphics.Drawing;
using OpenMLTD.MilliSim.Theater.Elements.Logical;
using OpenMLTD.MilliSim.Theater.Extensions;

namespace OpenMLTD.MilliSim.Theater.Elements.Visual.Overlays {
    public class SyncTimerOverlay : TextOverlay {

        public SyncTimerOverlay(GameBase game)
            : base(game) {
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
