using System.Diagnostics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using OpenMLTD.MilliSim.Extension.Components.CoreComponents;
using OpenMLTD.MilliSim.Extension.Components.ExtraComponents;
using OpenMLTD.MilliSim.Extension.Components.ScoreComponents.Overlays;
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

        private void SyncTimer_StateChanged(object sender, SyncTimerStateChangedEventArgs e) {
            if (e.NewState == MediaState.Stopped) {
                foreach (var sound in AudioManager.GetLoadedSounds()) {
                    sound.Source?.Stop();
                }

                var comboDisplay = this.FindSingleElement<ComboDisplay>();

                if (comboDisplay != null) {
                    comboDisplay.Opacity = 0f;
                }
            }

            UpdateStateDisplay();
        }

        private void TogglePlayState() {
            var syncTimer = this.FindSingleElement<SyncTimer>();

            Debug.Assert(syncTimer != null, nameof(syncTimer) + " != null");

            SetIsPlaying(!syncTimer.IsRunning, stop: false);
        }

        private void SetIsPlaying(bool isPlaying, bool stop) {
            var syncTimer = this.FindSingleElement<SyncTimer>();

            Debug.Assert(syncTimer != null, nameof(syncTimer) + " != null");

            if (!isPlaying) {
                if (stop) {
                    syncTimer.Stop();
                } else {
                    syncTimer.Pause();
                }
            } else {
                syncTimer.Start();
            }

            UpdateStateDisplay();
        }

        private void UpdateStateDisplay() {
            var syncTimer = this.FindSingleElement<SyncTimer>();

            Debug.Assert(syncTimer != null, nameof(syncTimer) + " != null");

            var helpOverlay = this.FindSingleElement<HelpOverlay>();

            if (helpOverlay != null) {
                helpOverlay.Visible = !syncTimer.IsRunning;
            }
        }

        private static readonly Keys[] EasterEggKeyStrokes = {
            Keys.D7, Keys.D6, Keys.D5, Keys.P, Keys.R, Keys.O
        };

        private int _easterEggIndex;

    }
}
