using System.Drawing;
using System.IO;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Rendering;
using OpenMLTD.MilliSim.Rendering.Extensions;
using OpenMLTD.MilliSim.Theater.Elements;
using OpenMLTD.MilliSim.Theater.Properties;

namespace OpenMLTD.MilliSim.Theater {
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
    public sealed partial class TheaterView : GameWindow {

        [UsedImplicitly(ImplicitUseKindFlags.Access)]
        public TheaterView(Game game)
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

        private void UnregisterEventHandlers() {
            KeyDown -= TheaterStage_KeyDown;
            Load -= TheaterStage_Load;

            var theaterDays = GetTypedGame();

            var video = theaterDays.Elements.FindOrNull<BackgroundVideo>();
            if (video != null) {
                video.VideoStateChanged -= Video_VideoStateChanged;
            }
        }

        private void RegisterEventHandlers() {
            KeyDown += TheaterStage_KeyDown;
            Load += TheaterStage_Load;

            var theaterDays = GetTypedGame();

            var video = theaterDays.Elements.FindOrNull<BackgroundVideo>();
            if (video != null) {
                video.VideoStateChanged += Video_VideoStateChanged;
            }
        }

        private void InitializeSettings() {
            var settings = Program.Settings;

            Text = string.Format(TitleTemplate, settings.Game.Title);

            Icon = Resources.MLTD_Icon;

            ClientSize = new Size(settings.Window.Width, settings.Window.Height);
            CenterToScreen();
        }

        private void InitializeElements() {
            var settings = Program.Settings;
            var theaterDays = GetTypedGame();

            var video = theaterDays.Elements.FindOrNull<BackgroundVideo>();
            if (video != null) {
                var animFileName = Path.GetFileName(settings.Media.BackgroundAnimation);
                var debugOverlay = theaterDays.Elements.Find<DebugOverlay>();
                debugOverlay.Text = $"Background animation:\n{animFileName}";

                theaterDays.Invoke(() => {
                    video.OpenFile(settings.Media.BackgroundAnimation);
                    if (!video.IsReadyToPlay) {
                        video.ReadyToPlayEvent.WaitOne();
                    }
                    if (!video.CanPlay) {
                        debugOverlay.Text = $"Error:\nunable to play <{animFileName}>.";
                        debugOverlay.Show();
                    }
                    video.PauseOnFirstFrame();
                });
            }

            var image = theaterDays.Elements.FindOrNull<BackgroundImage>();
            if (image != null) {
                image.Load(settings.Media.BackgroundImage);
            }
        }

        private static readonly string TitleTemplate = "MilliSim: {0}";

        private TheaterDays GetTypedGame() => (TheaterDays)Game;

    }
}
