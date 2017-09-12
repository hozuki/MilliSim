namespace OpenMLTD.MilliSim.Core.Entities.Runtime {
    public sealed class RuntimeNote {

        public int ID { get; set; }

        /// <summary>
        /// Absolute hit time, in seconds.
        /// </summary>
        public double HitTime { get; set; }

        /// <summary>
        /// The duration, starting from this note enters the stage, to this note exits the stage (reaches <see cref="HitTime"/>), in seconds.
        /// The actual value will be modified with <see cref="RelativeSpeed"/>.
        /// </summary>
        public double LeadTime { get; set; }

        public float StartX { get; set; }

        public float EndX { get; set; }

        public float RelativeSpeed { get; set; }

        public RuntimeNoteType Type { get; set; }

        public FlickDirection FlickDirection { get; set; }

        public RuntimeNoteSize Size { get; set; }

        public int GroupID { get; set; }

        public RuntimeNote NextSync { get; set; }

        public RuntimeNote PrevSync { get; set; }

        public RuntimeNote PrevHold { get; set; }

        public RuntimeNote NextHold { get; set; }

        public RuntimeNote PrevFlick { get; set; }

        public RuntimeNote NextFlick { get; set; }

        public RuntimeNote NextSlide { get; set; }

        public RuntimeNote PrevSlide { get; set; }

    }
}
