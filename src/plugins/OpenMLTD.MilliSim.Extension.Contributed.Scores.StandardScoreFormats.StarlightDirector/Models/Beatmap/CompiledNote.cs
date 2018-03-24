namespace OpenMLTD.MilliSim.Extension.Contributed.Scores.StandardScoreFormats.StarlightDirector.Models.Beatmap {
    public sealed class CompiledNote {

        public int ID { get; internal set; }

        public NoteType Type { get; internal set; }

        public double HitTiming { get; internal set; }

        public NotePosition StartPosition { get; internal set; }

        public NotePosition FinishPosition { get; internal set; }

        public NoteFlickType FlickType { get; internal set; }

        public bool IsSync { get; internal set; }

        public int GroupID { get; internal set; }

    }
}
