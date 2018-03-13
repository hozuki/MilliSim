using System.Diagnostics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using OpenMLTD.MilliSim.Extension.Components.CoreComponents;
using OpenMLTD.MilliSim.Extension.Components.ExtraComponents;
using OpenMLTD.MilliSim.Foundation.Extensions;

namespace OpenMLTD.TheaterDays {
    partial class Theater {

        private void KeyboardStateHandler_KeyDown(object sender, KeyEventArgs e) {
            switch (e.KeyCode) {
                case Keys.Space:
                    TogglePlayState();
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

#if DEBUG
            Debug.Print("Key down: " + e.KeyCode.ToString());
#endif

            if (_easterEggIndex >= EasterEggKeyStrokes.Length) {
                _easterEggIndex = 0;

                var cuties = this.FindSingleElement<CuteIdol>();
                if (cuties != null) {
                    cuties.PickRandomCharacter();
                    cuties.Visible = true;
                }
            }
        }

        private void KeyboardStateHandler_KeyUp(object sender, KeyEventArgs e) {
        }

        private void BackgroundMedia_PlaybackStopped(object sender, System.EventArgs e) {
            SetIsPlaying(false, stop: true);
        }

        private void TogglePlayState() {
            SetIsPlaying(!_isPlaying, stop: false);
        }

        private void SetIsPlaying(bool isPlaying, bool stop) {
            var bga = this.FindSingleElement<BackgroundVideo>();
            var bgm = this.FindSingleElement<BackgroundMusic>();
            var syncTimer = this.FindSingleElement<SyncTimer>();

            Debug.Assert(syncTimer != null, nameof(syncTimer) + " != null");

            _isPlaying = isPlaying;

            if (!isPlaying) {
                if (stop) {
                    bga?.Stop();
                    bgm?.Music?.Source.Stop();
                    syncTimer.Stopwatch.Reset();
                } else {
                    bga?.Pause();
                    bgm?.Music?.Source.Pause();
                    syncTimer.Stopwatch.Stop();
                }
            } else {
                if (bga != null) {
                    if (bga.State == MediaState.Stopped) {
                        bga.Play();
                    } else {
                        bga.Resume();
                    }
                }

                bgm?.Music?.Source.PlayDirect();

                syncTimer.Stopwatch.Start();
            }

            UpdateStateDisplay();
        }

        private void UpdateStateDisplay() {
            var isPlaying = _isPlaying;

            var helpOverlay = this.FindSingleElement<HelpOverlay>();

            if (helpOverlay != null) {
                helpOverlay.Visible = !isPlaying;
            }
        }

        private static readonly Keys[] EasterEggKeyStrokes = {
            Keys.D7, Keys.D6, Keys.D5, Keys.P, Keys.R, Keys.O
        };

        private int _easterEggIndex;

        private bool _isPlaying;

    }
}
