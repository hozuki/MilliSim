namespace OpenMLTD.MilliSim.Core.Entities.Runtime {
    public sealed class RuntimeNote {

        internal RuntimeNote() {
        }

        public int ID { get; internal set; }

        /// <summary>
        /// Absolute hit time, in seconds.
        /// </summary>
        public double HitTime { get; internal set; }

        /// <summary>
        /// The duration, starting from this note enters the stage, to this note exits the stage (reaches <see cref="HitTime"/>), in seconds.
        /// The actual value will be modified with <see cref="RelativeSpeed"/>.
        /// </summary>
        public double LeadTime { get; internal set; }

        public float StartX { get; internal set; }

        public float EndX { get; internal set; }

        public float RelativeSpeed { get; internal set; }

        public RuntimeNoteType Type { get; internal set; }

        public FlickDirection FlickDirection { get; internal set; }

        public RuntimeNoteSize Size { get; internal set; }

        public int GroupID { get; internal set; }

        public RuntimeNote NextSync { get; internal set; }

        public RuntimeNote PrevSync { get; internal set; }

        public RuntimeNote PrevHold { get; internal set; }

        public RuntimeNote NextHold { get; internal set; }

        public RuntimeNote PrevFlick { get; internal set; }

        public RuntimeNote NextFlick { get; internal set; }

        public RuntimeNote NextSlide { get; internal set; }

        public RuntimeNote PrevSlide { get; internal set; }

    }
}
