using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using OpenMLTD.MilliSim.Graphics.Extensions;
using OpenMLTD.MilliSim.Theater.Elements;
using OpenMLTD.MilliSim.Theater.Elements.Logical;
using OpenMLTD.MilliSim.Theater.Elements.Visual.Background;
using OpenMLTD.MilliSim.Theater.Elements.Visual.Gaming;
using OpenMLTD.MilliSim.Theater.Elements.Visual.Overlays;
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
        }

        private void TheaterStage_StageReady(object sender, EventArgs e) {
            var settings = Program.Settings;
            var theaterDays = Game.AsTheaterDays();

            var scoreLoader = theaterDays.FindSingleElement<ScoreLoader>();
            if (scoreLoader == null) {
                throw new InvalidOperationException();
            }
            var tapPoints = theaterDays.FindSingleElement<TapPoints>();
            if (tapPoints == null) {
                throw new InvalidOperationException();
            }
            if (scoreLoader.RuntimeScore != null) {
                tapPoints.TrackCount = scoreLoader.RuntimeScore.TrackCount;
                tapPoints.PerformLayout();
            }

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
            } else {
                var help = theaterDays.FindSingleElement<HelpOverlay>();
                if (help != null) {
                    help.Show();
                }
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

            switch (e.NewState) {
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
                switch (e.NewState) {
                    case MediaEngineEvent.Play:
                        music.Play();
                        if (e.OldValidState == MediaEngineEvent.Pause) {
                            if (video != null) {
                                // Force seeking to music time to avoid the lag.
                                // The lag appears when you pause for too many times (about 5 times) during playback.
                                video.CurrentTime = music.CurrentTime;
                            }
                        }
                        break;
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
                        theaterDays.AudioManager.Sfx.StopAll();
                        break;
                }
            }

            var tapPoints = theaterDays.FindSingleElement<TapPoints>();
            if (tapPoints != null) {
                switch (e.NewState) {
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
            var settings = Program.Settings;

            var video = theaterDays.FindSingleElement<BackgroundVideo>();
            var audio = theaterDays.FindSingleElement<AudioController>();
            var music = audio?.Music;
            var help = theaterDays.FindSingleElement<HelpOverlay>();

            switch (e.KeyCode) {
                case Keys.Space:
                    if (video != null) {
                        if (video.IsStopped) {
                            video.Play();
                        } else {
                            video.TogglePause();
                        }
                    } else {
                        if (music != null) {
                            if (music.IsPlaying) {
                                music.Pause();
                            } else {
                                music.Play();
                            }
                            if (help != null) {
                                help.Visible = !music.IsPlaying;
                            }
                        } else {
                            if (help != null) {
                                help.Visible = !help.Visible;
                            }
                        }
                    }
                    break;
                case Keys.F2:
                    if (music != null) {
                        music.Stop();
                    }

                    if (video != null) {
                        video.PauseOnFirstFrame();
                    } else {
                        if (help != null) {
                            help.Show();
                        }
                    }

                    theaterDays.AudioManager.Sfx.StopAll();
                    break;
                case Keys.P:
                    if (theaterDays.IsSuspended) {
                        theaterDays.Resume();
                    } else {
                        theaterDays.Suspend();
                    }
                    break;
                case Keys.Right:
                case Keys.Left: {
                        var timer = theaterDays.FindSingleElement<SyncTimer>();
                        if (timer != null) {
                            var current = timer.CurrentTime;
                            var next = e.KeyCode == Keys.Right ? current + TimeSpan.FromSeconds(5) : current - TimeSpan.FromSeconds(5);
                            if (video != null) {
                                video.CurrentTime = next;
                            }
                            if (music != null) {
                                music.CurrentTime = next;
                            }
                            theaterDays.AudioManager.Sfx.StopAll();
                        }
                    }
                    break;
                case Keys.Q:
                    var newTime = TimeSpan.FromMinutes(2);
                    if (music != null) {
                        music.CurrentTime = newTime;
                    }
                    if (video != null) {
                        video.CurrentTime = newTime;
                    }
                    break;
                case Keys.I:
                    theaterDays.AudioManager.Sfx.Play(settings.Sfx.Tap.Perfect, Program.PluginManager.AudioFormats);
                    break;
#if DEBUG
                case Keys.D:
                    GlobalDebug.Enabled = !GlobalDebug.Enabled;
                    break;
#endif
            }
        }

    }
}
