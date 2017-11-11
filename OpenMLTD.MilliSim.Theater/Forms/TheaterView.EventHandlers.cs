using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using OpenMLTD.MilliSim.Audio;
using OpenMLTD.MilliSim.Extension.Components.CoreComponents;
using OpenMLTD.MilliSim.Extension.Components.CoreComponents.Configuration;
using OpenMLTD.MilliSim.Extension.Components.ExtraComponents;
using OpenMLTD.MilliSim.Extension.Components.ScoreComponents;
using OpenMLTD.MilliSim.Extension.Components.ScoreComponents.Configuration;
using OpenMLTD.MilliSim.Extension.Components.ScoreComponents.Gaming;
using OpenMLTD.MilliSim.GameAbstraction.Extensions;
using OpenMLTD.MilliSim.Graphics.Extensions;
using OpenMLTD.MilliSim.Theater.Configuration;
using OpenMLTD.MilliSim.Theater.Extensions;
using OpenMLTD.MilliSim.Theater.Properties;
using SharpDX.MediaFoundation;

namespace OpenMLTD.MilliSim.Theater.Forms {
    partial class TheaterView {

        private void TheaterStage_Load(object sender, EventArgs e) {
            var theaterDays = Game.AsTheaterDays();

            var scoreLoaderConfig = theaterDays.ConfigurationStore.Get<ScoreLoaderConfig>();
            var mainAppConfig = theaterDays.ConfigurationStore.Get<MainAppConfig>();

            Text = string.Format(TitleTemplate, ApplicationHelper.GetCodeName(), scoreLoaderConfig.Data.Title);

            Icon = Resources.MLTD_Icon;

            ClientSize = new Size(mainAppConfig.Data.Window.Width, mainAppConfig.Data.Window.Height);
            CenterToScreen();

            // Register element events.
            var video = theaterDays.FindSingleElement<BackgroundVideo>();
            if (video != null) {
                video.VideoStateChanged += Video_VideoStateChanged;
            }
        }

        private void TheaterStage_StageReady(object sender, EventArgs e) {
            var theaterDays = Game.AsTheaterDays();
            var store = theaterDays.ConfigurationStore;

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
                var audioControllerConfig = store.Get<AudioControllerConfig>();
                var musicFileName = Path.GetFileName(audioControllerConfig.Data.BackgroundMusic);
                if (debugOverlay != null) {
                    debugOverlay.AddLine($"Background music: {musicFileName}");
                }
            }

            var video = theaterDays.FindSingleElement<BackgroundVideo>();
            if (video != null) {
                var backgroundVideoConfig = store.Get<BackgroundVideoConfig>();
                var animFileName = Path.GetFileName(backgroundVideoConfig.Data.BackgroundVideo);
                if (debugOverlay != null) {
                    debugOverlay.AddLine($"Background animation: {animFileName}");
                }

                theaterDays.Invoke(() => {
                    video.OpenFile(backgroundVideoConfig.Data.BackgroundVideo);
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
                var backgroundImageConfig = store.Get<BackgroundImageConfig>();
                image.Load(backgroundImageConfig.Data.BackgroundImage);
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
                        music.Source.Play();
                        if (e.OldValidState == MediaEngineEvent.Pause) {
                            if (video != null) {
                                // Force seeking to music time to avoid the lag.
                                // The lag appears when you pause for too many times (about 5 times) during playback.
                                video.CurrentTime = music.Source.CurrentTime;
                            }
                        }
                        break;
                    case MediaEngineEvent.Playing:
                        music.Source.Play();
                        break;
                    case MediaEngineEvent.Pause:
                        music.Source.Pause();
                        break;
                    case MediaEngineEvent.CanPlay:
                    case MediaEngineEvent.Ended:
                    case MediaEngineEvent.Error:
                    case MediaEngineEvent.Abort:
                        music.Source.Stop();
                        theaterDays.AudioManager.StopAll();
                        break;
                }
            }

            var tapPoints = theaterDays.FindSingleElement<TapPoints>();
            if (tapPoints != null) {
                switch (e.NewState) {
                    case MediaEngineEvent.Play:
                    case MediaEngineEvent.Playing:
                        break;
                    case MediaEngineEvent.Pause:
                    case MediaEngineEvent.Ended:
                    case MediaEngineEvent.Error:
                    case MediaEngineEvent.Abort:
                        break;
                }
            }
        }

        private void TheaterStage_KeyDown(object sender, KeyEventArgs e) {
            var theaterDays = Game.AsTheaterDays();

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
                            if (music.Source.State == AudioState.Playing) {
                                music.Source.Pause();
                            } else {
                                music.Source.Play();
                            }
                            if (help != null) {
                                help.Visible = music.Source.State != AudioState.Playing;
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
                        music.Source.Stop();
                    }

                    if (video != null) {
                        video.PauseOnFirstFrame();
                    } else {
                        if (help != null) {
                            help.Show();
                        }
                    }

                    theaterDays.AudioManager.StopAll();
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
                                music.Source.CurrentTime = next;
                                theaterDays.AudioManager.StopAllExcept(music);
                            } else {
                                theaterDays.AudioManager.StopAll();
                            }
                        }
                    }
                    break;
                case Keys.Q:
                    var newTime = TimeSpan.FromMinutes(2);
                    if (music != null) {
                        music.Source.CurrentTime = newTime;
                    }
                    if (video != null) {
                        video.CurrentTime = newTime;
                    }
                    break;
#if DEBUG
                case Keys.D:
                    GlobalDebug.Enabled = !GlobalDebug.Enabled;
                    break;
#endif
                case Keys.F1:
                    using (var form = new AboutWindow(theaterDays)) {
                        form.ShowDialog(this);
                    }
                    break;
            }

            if (e.KeyCode == EasterEggKeyStrokes[_easterEggIndex]) {
                ++_easterEggIndex;
            } else {
                _easterEggIndex = 0;
                if (e.KeyCode == EasterEggKeyStrokes[_easterEggIndex]) {
                    ++_easterEggIndex;
                }
            }

            if (_easterEggIndex >= EasterEggKeyStrokes.Length) {
                _easterEggIndex = 0;

                var cuties = theaterDays.FindSingleElement<CuteIdol>();
                if (cuties != null) {
                    cuties.PickRandomCharacter();
                    cuties.Visible = true;
                }
            }
        }

        private int _easterEggIndex;

        private static readonly Keys[] EasterEggKeyStrokes = {
            Keys.M, Keys.I, Keys.L, Keys.L, Keys.I, Keys.O, Keys.N, Keys.L, Keys.I, Keys.V, Keys.E,
            Keys.D7, Keys.D6, Keys.D5, Keys.P, Keys.R, Keys.O
        };

    }
}
