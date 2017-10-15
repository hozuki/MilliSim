using System.Windows.Forms;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Theater.Elements.Visual.Background;
using OpenMLTD.MilliSim.Theater.Extensions;

namespace OpenMLTD.MilliSim.Theater.Forms {
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
    public sealed partial class TheaterView : GameWindow {

        [UsedImplicitly(ImplicitUseKindFlags.Access)]
        public TheaterView(GameBase game)
            : base(game) {
            RegisterEventHandlers();
        }

        ~TheaterView() {
            UnregisterEventHandlers();
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                UnregisterEventHandlers();
            }
            base.Dispose(disposing);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
            if (keyData == Keys.Escape) {
                Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void UnregisterEventHandlers() {
            KeyDown -= TheaterStage_KeyDown;
            StageReady -= TheaterStage_StageReady;
            Load -= TheaterStage_Load;

            var theaterDays = Game.AsTheaterDays();

            var video = theaterDays.FindSingleElement<BackgroundVideo>();
            if (video != null) {
                video.VideoStateChanged -= Video_VideoStateChanged;
            }
        }

        private void RegisterEventHandlers() {
            KeyDown += TheaterStage_KeyDown;
            StageReady += TheaterStage_StageReady;
            Load += TheaterStage_Load;
        }

        private static readonly string TitleTemplate = "MilliSim (\"{0}\"): {1}";

    }
}
