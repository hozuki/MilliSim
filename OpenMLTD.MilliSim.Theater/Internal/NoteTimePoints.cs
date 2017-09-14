namespace OpenMLTD.MilliSim.Theater.Internal {
    public struct NoteTimePoints {

        public NoteTimePoints(double enter, double leave) {
            Enter = enter;
            Leave = leave;
            Duration = leave - enter;
        }

        public double Enter { get; set; }

        public double Leave { get; set; }

        public double Duration { get; set; }

    }
}
