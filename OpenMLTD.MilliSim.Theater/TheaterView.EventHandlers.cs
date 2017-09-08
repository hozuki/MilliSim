using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using OpenMLTD.MilliSim.Core.Entities;
using OpenMLTD.MilliSim.Graphics.Extensions;
using OpenMLTD.MilliSim.Theater.Elements;
using OpenMLTD.MilliSim.Theater.Extensions;
using OpenMLTD.MilliSim.Theater.Properties;
using SharpDX.MediaFoundation;

namespace OpenMLTD.MilliSim.Theater {
    partial class TheaterView {

        private void TheaterStage_Load(object sender, EventArgs e) {
            var settings = Program.Settings;

            Text = string.Format(TitleTemplate, settings.Game.Title);

            Icon = Resources.MLTD_Icon;

            ClientSize = new Size(settings.Window.Width, settings.Window.Height);
            CenterToScreen();

            // Register element events.
            var theaterDays = Game.AsTheaterDays();

            var video = theaterDays.FindSingleElement<BackgroundVideo>();
            if (video != null) {
                video.VideoStateChanged += Video_VideoStateChanged;
            }

            var tapPoints = theaterDays.FindSingleElement<TapPoints>();
            if (tapPoints != null) {
                tapPoints.TrackCount = TrackHelper.GetTrackCount(settings.Game.Difficulty);
            }
        }

        private void TheaterStage_StageReady(object sender, EventArgs e) {
            var settings = Program.Settings;
            var theaterDays = Game.AsTheaterDays();

            var debugOverlay = theaterDays.FindSingleElement<DebugOverlay>();

            var audioController = theaterDays.FindSingleElement<AudioController>();
            if (audioController?.Music != null) {
                var musicFileName = Path.GetFileName(settings.Media.BackgroundMusic);
                if (debugOverlay != null) {
                    debugOverlay.AddLine($"Background music: {musicFileName}");
                }
            }

            var video = theaterDays.FindSingleElement<BackgroundVideo>();
            if (video != null) {
                var animFileName = Path.GetFileName(settings.Media.BackgroundAnimation);
                if (debugOverlay != null) {
                    debugOverlay.AddLine($"Background animation: {animFileName}");
                }

                theaterDays.Invoke(() => {
                    video.OpenFile(settings.Media.BackgroundAnimation);
                    if (!video.CanPlay) {
                        if (debugOverlay != null) {
                            debugOverlay.AddLine($"ERROR: Unable to play <{animFileName}>. File type is not supported.");
                            debugOverlay.Show();
                        }
                    } else {
                        // Omitting these 2 lines will result in a black screen.
                        // (When the video is stopped, it renders nothing.)
                        video.WaitUntilReady();
                        video.PauseOnFirstFrame();
                    }
                });
            }

            var image = theaterDays.FindSingleElement<BackgroundImage>();
            if (image != null) {
                image.Load(settings.Media.BackgroundImage);
            }
        }

        private void Video_VideoStateChanged(object sender, VideoStateChangedEventArgs e) {
            var theaterDays = Game.AsTheaterDays();
            var helpOverlay = theaterDays.FindSingleElement<HelpOverlay>();
            var debugOverlay = theaterDays.FindSingleElement<DebugOverlay>();
            var video = theaterDays.FindSingleElement<BackgroundVideo>();

            switch (e.Event) {
                case MediaEngineEvent.Play:
                case MediaEngineEvent.Playing:
                    if (helpOverlay != null) {
                        helpOverlay.Hide();
                    }
                    break;
                case MediaEngineEvent.CanPlay:
                case MediaEngineEvent.Pause:
                case MediaEngineEvent.Ended:
                    if (helpOverlay != null) {
                        helpOverlay.Show();
                    }
                    break;
                case MediaEngineEvent.Error:
                    if (debugOverlay != null) {
                        if (video != null) {
                            var error = video.GetError();
                            var errCode = (MediaEngineErr)error.GetErrorCode();
                            var hr = error.ExtendedErrorCode;
                            debugOverlay.AddLine($"ERROR: MediaEngine reported an error (Type: {errCode}, {hr})");
                        } else {
                            // Not reachable.
                            debugOverlay.AddLine("ERROR: MediaEngine reported an error.");
                        }
                        debugOverlay.Show();
                    }
                    if (helpOverlay != null) {
                        helpOverlay.Hide();
                    }
                    break;
                case MediaEngineEvent.Abort:
                    if (debugOverlay != null) {
                        debugOverlay.AddLine("Warning: MediaEngine aborted.");
                        debugOverlay.Show();
                    }
                    if (helpOverlay != null) {
                        helpOverlay.Hide();
                    }
                    break;
            }

            var audio = theaterDays.FindSingleElement<AudioController>();
            var music = audio?.Music;
            if (music != null) {
                switch (e.Event) {
                    case MediaEngineEvent.Play:
                    case MediaEngineEvent.Playing:
                        music.Play();
                        break;
                    case MediaEngineEvent.Pause:
                        music.Pause();
                        break;
                    case MediaEngineEvent.CanPlay:
                    case MediaEngineEvent.Ended:
                    case MediaEngineEvent.Error:
                    case MediaEngineEvent.Abort:
                        music.Stop();
                        break;
                }
            }

            var tapPoints = theaterDays.FindSingleElement<TapPoints>();
            if (tapPoints != null) {
                switch (e.Event) {
                    case MediaEngineEvent.Play:
                    case MediaEngineEvent.Playing:
                        tapPoints.FadeIn(TimeSpan.FromSeconds(1.5));
                        break;
                    case MediaEngineEvent.Pause:
                    case MediaEngineEvent.Ended:
                    case MediaEngineEvent.Error:
                    case MediaEngineEvent.Abort:
                        tapPoints.FadeOut(TimeSpan.FromSeconds(1.5));
                        break;
                }
            }
        }

        private void TheaterStage_KeyDown(object sender, KeyEventArgs e) {
            var theaterDays = Game.AsTheaterDays();

            var video = theaterDays.FindSingleElement<BackgroundVideo>();
            var audio = theaterDays.FindSingleElement<AudioController>();
            var music = audio?.Music;
            switch (e.KeyCode) {
                case Keys.Space:
                    if (video != null) {
                        if (video.IsStopped) {
                            video.Play();
                        } else {
                            video.TogglePause();
                        }
                    }
                    break;
                case Keys.F2:
                    if (music != null) {
                        music.Stop();
                    }

                    if (video != null) {
                        video.PauseOnFirstFrame();
                    }
                    break;
                case Keys.P:
                    if (theaterDays.IsSuspended) {
                        theaterDays.Resume();
                    } else {
                        theaterDays.Suspend();
                    }
                    break;
                case Keys.Up:
                    if (music != null) {
                        music.Volume += 0.05f;
                    }
                    break;
                case Keys.Down:
                    if (music != null) {
                        music.Volume -= 0.05f;
                    }
                    break;
                case Keys.Q:
                    var newTime = TimeSpan.FromMinutes(1) + TimeSpan.FromSeconds(45);
                    if (music != null) {
                        music.CurrentTime = newTime;
                    }
                    if (video != null) {
                        video.CurrentTime = newTime;
                    }
                    break;
            }
        }

    }
}
