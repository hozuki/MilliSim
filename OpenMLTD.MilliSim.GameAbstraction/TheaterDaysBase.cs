using OpenMLTD.MilliSim.Audio;
using OpenMLTD.MilliSim.Graphics;

namespace OpenMLTD.MilliSim.GameAbstraction {
    public abstract class TheaterDaysBase : VisualGame {

        public AudioManager AudioManager => (AudioManager)BaseAudioManager;

        public PluginManager PluginManager { get; internal set; }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                PluginManager?.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}
