namespace OpenMLTD.MilliSim.Theater.Configuration {
    public sealed class AnimationClass {

        public AnimationStages SongTitle { get; set; }

        public AnimationStages SongTitleReappear { get; set; }

        public sealed class AnimationStages {

            public double Enter { get; set; }

            public double FadeIn { get; set; }

            public double Hold { get; set; }

            public double FadeOut { get; set; }

        }

    }
}
