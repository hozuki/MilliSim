using OpenMLTD.MilliSim.Theater.Configuration.Primitives;

namespace OpenMLTD.MilliSim.Theater.Configuration {
    public sealed class SfxClass {

        public NoteSfxGroup Tap { get; set; }

        public NoteSfxGroup Hold { get; set; }

        public NoteSfxGroup Flick { get; set; }

        public NoteSfxGroup Slide { get; set; }

        public string SlideHold { get; set; }

        public string HoldHold { get; set; }

        public NoteSfxGroup SlideEnd { get; set; }

        public NoteSfxGroup HoldEnd { get; set; }

        public NoteSfxGroup Special { get; set; }

        public string SpecialEnd { get; set; }

        public string SpecialHold { get; set; }

        public string[] Shouts { get; set; }

    }
}
