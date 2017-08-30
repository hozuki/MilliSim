using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace OpenMLTD.MilliSim.Core.Entities {
    public sealed class Note : NoteBase {

        /// <summary>
        /// Base type of this note.
        /// </summary>
        public NoteType Type { get; set; }

        void a() {
            Console.WriteLine();
        }

        /// <summary>
        /// Start position of this note.
        /// </summary>
        /// <remarks>
        /// <para>From https://github.com/thiEFcat/ScrObjAnalyzer:</para>
        /// <para>Ranges from -1 to 2 in 2Mix/2Mix+ mode, -2 to 5 in 4Mix mode, and -3 to 8 in 6Mix/Million Mix mode.
        /// Allows decimal number (maybe).
        /// UPDATE 170812: Start point -3 has been discovered in official 'Shooting Stars' beatmap.</para>
        /// </remarks>
        public float StartPosition { get; set; }

        /// <summary>
        /// End position of this note.
        /// </summary>
        /// <remarks>
        /// <para>From https://github.com/thiEFcat/ScrObjAnalyzer:</para>
        /// <para>Ranges from 0 to n-1 in nMix mode.</para>
        /// </remarks>
        public float EndPosition { get; set; }

        /// <summary>
        /// Falling speed of this note.
        /// </summary>
        /// <remarks>
        /// There is also a speed table at the end of each beatmap. Not sure how that mapping goes.
        /// </remarks>
        public float Speed { get; set; }

        /// <summary>
        /// Unknown.
        /// </summary>
        public int Duration { get; set; }

        /// <summary>
        /// <see cref="PolyPoint"/> data of this note. This property must not be null if the type of this note is
        /// <see cref="NoteType.SlideSmall"/>.
        /// </summary>
        [CanBeNull, ItemNotNull]
        public IReadOnlyList<PolyPoint> PolyPoints { get; set; }

        /// <summary>
        /// Ending type of this note.
        /// </summary>
        public NoteEndType EndType { get; set; }

        /// <summary>
        /// Unknown.
        /// </summary>
        public double LeadTime { get; set; }

    }
}
