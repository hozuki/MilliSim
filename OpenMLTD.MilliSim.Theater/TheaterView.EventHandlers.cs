using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
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
            var theaterDays = GetTypedGame();

            var video = theaterDays.GetBackgroundVideo();
            if (video != null) {
                video.VideoStateChanged += Video_VideoStateChanged;
            }
        }

        private void TheaterStage_StageReady(object sender, EventArgs e) {
            var settings = Program.Settings;
            var theaterDays = GetTypedGame();

            var debugOverlay = theaterDays.GetDebugOverlay();

            var audioController = theaterDays.GetAudioController();
            if (audioController?.Music != null) {
                var musicFileName = Path.GetFileName(settings.Media.BackgroundMusic);
                if (debugOverlay != null) {
                    debugOverlay.AddLine($"Background music: {musicFileName}");
                }
            }

            var video = theaterDays.GetBackgroundVideo();
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

            var image = theaterDays.GetBackgroundImage();
            if (image != null) {
                image.Load(settings.Media.BackgroundImage);
            }
        }

        private void Video_VideoStateChanged(object sender, VideoStateChangedEventArgs e) {
            var theaterDays = GetTypedGame();
            var helpOverlay = theaterDays.GetHelpOverlay();
            var debugOverlay = theaterDays.GetDebugOverlay();
            var video = theaterDays.GetBackgroundVideo();

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
                            debugOverlay.Text = $"Error: MediaEngine reported an error.\nType: {errCode}\nExtended: {hr}";
                        } else {
                            // Not reachable.
                            debugOverlay.Text = "Error: MediaEngine reported an error.";
                        }
                        debugOverlay.Show();
                    }
                    if (helpOverlay != null) {
                        helpOverlay.Hide();
                    }
                    break;
                case MediaEngineEvent.Abort:
                    if (debugOverlay != null) {
                        debugOverlay.Text = "Warning: MediaEngine aborted.";
                        debugOverlay.Show();
                    }
                    if (helpOverlay != null) {
                        helpOverlay.Hide();
                    }
                    break;
            }
        }

        private void TheaterStage_KeyDown(object sender, KeyEventArgs e) {
            var theaterDays = GetTypedGame();

            switch (e.KeyCode) {
                case Keys.Space: {
                        var video = theaterDays.GetBackgroundVideo();
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
                        var video = theaterDays.GetBackgroundVideo();
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
                case Keys.B: {
                        var audio = theaterDays.GetAudioController();
                        var music = audio?.Music;
                        if (music != null) {
                            if (music.IsPlaying) {
                                music.Pause();
                            } else {
                                music.Play();
                            }
                        }
                        break;
                    }
                case Keys.Up: {
                        var audio = theaterDays.GetAudioController();
                        var music = audio?.Music;
                        if (music != null) {
                            music.Volume += 0.05f;
                        }
                        break;
                    }
                case Keys.Down: {
                        var audio = theaterDays.GetAudioController();
                        var music = audio?.Music;
                        if (music != null) {
                            music.Volume -= 0.05f;
                        }
                        break;
                    }
            }
        }

    }
}
