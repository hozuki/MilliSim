using OpenMLTD.MilliSim.Configuration.Entities;

namespace OpenMLTD.MilliSim.Extension.Components.ScoreComponents.Configuration {
    public sealed class SlideMotionConfig : ConfigBase {

        public SlideMotionConfigData Data { get; set; }

        public sealed class SlideMotionConfigData {

            public SlideMotionIcon Icon { get; set; }

            public SlideMotionPosition Position { get; set; }

        }

        public enum SlideMotionIcon {

            None = 0,
            TapPoint = 1,
            SlideStart = 2,
            SlideMiddle = 3,
            SlideEnd = 4

        }

        public enum SlideMotionPosition {

            Below = 0,
            Above = 1

        }

    }
}
