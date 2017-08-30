using System;
using System.Windows.Forms;
using OpenMLTD.MilliSim.Rendering.Extensions;
using OpenMLTD.MilliSim.Theater.Elements;
using SharpDX.MediaFoundation;

namespace OpenMLTD.MilliSim.Theater {
    partial class TheaterView {

        private void TheaterStage_Load(object sender, EventArgs e) {
            InitializeSettings();
            InitializeElements();
        }

        private void Video_VideoStateChanged(object sender, VideoStateChangedEventArgs e) {
            var theaterDays = GetTypedGame();
            var helpOverlay = theaterDays.Elements.Find<HelpOverlay>();

            switch (e.Event) {
                case MediaEngineEvent.Play:
                case MediaEngineEvent.Playing:
                    helpOverlay.Visible = false;
                    break;
                case MediaEngineEvent.CanPlay:
                case MediaEngineEvent.Pause:
                case MediaEngineEvent.Ended:
                case MediaEngineEvent.Error:
                case MediaEngineEvent.Abort:
                    helpOverlay.Visible = true;
                    break;
            }
        }

        private void TheaterStage_KeyDown(object sender, KeyEventArgs e) {
            var theaterDays = GetTypedGame();

            switch (e.KeyCode) {
                case Keys.Space: {
                        var video = theaterDays.Elements.FindOrNull<BackgroundVideo>();
                        if (video != null) {
                            if (video.IsStopped) {
                                video.Play();
                            } else {
                                video.TogglePause();
                            }
                        }
                        break;
                    }
                case Keys.F2: {
                        var video = theaterDays.Elements.FindOrNull<BackgroundVideo>();
                        if (video != null) {
                            video.PauseOnFirstFrame();
                        }
                        break;
                    }
                case Keys.P: {
                        if (theaterDays.IsSuspended) {
                            theaterDays.Resume();
                        } else {
                            theaterDays.Suspend();
                        }
                        break;
                    }
            }
        }

    }
}
