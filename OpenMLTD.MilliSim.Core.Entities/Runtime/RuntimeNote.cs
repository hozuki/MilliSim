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

        /// <summary>
        /// Ticks. Use <see cref="long"/> to avoid floating point loss in equality comparison.
        /// You must set this value along with <see cref="HitTime"/>, because this field is also used for
        /// detecting sync notes.
        /// </summary>
        /// <remarks>
        /// <para>
        /// I found this funny and annoying. The dev guys (in BandaiNamco, namingly) chose 1120 as the tick
        /// factor (see <see cref="RuntimeNoteHelper.TicksToSeconds"/>). However, after being converted to
        /// seconds, you cannot simply compare the equality of two notes' hit timing because THE VALUES ARE
        /// SIMPLY NOT RECOGNIZED AS EQUAL BY <see cref="double.Equals(double)"/>! WHAT THE HELL? If you
        /// actually want to mark these notes as sync, you must preserve the original <see cref="Note.Tick"/>
        /// value, and compare the tick values, since they are (long) integers and always accurate. The
        /// delta between correct (represented by <see cref="HitTime"/>) and recognized (represented by <see cref="Ticks"/>)
        /// values are often less than 0.02 seconds. It is acceptable for rhythm games on smartphones and pads,
        /// true. But in fact they are not equal.
        /// </para>
        /// <para>
        /// Be happy, devs of Cygames! I found a worse opponent for you!
        /// </para>
        /// </remarks>
        public long Ticks { get; set; }

    }
}
