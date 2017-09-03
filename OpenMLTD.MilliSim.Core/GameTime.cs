using System;

namespace OpenMLTD.MilliSim.Core {
    public sealed class GameTime {

        public GameTime(TimeSpan delta, TimeSpan total) {
            Delta = delta;
            Total = total;
        }

        public TimeSpan Delta { get; }

        public TimeSpan Total { get; }

        public static readonly GameTime Zero = new GameTime(TimeSpan.Zero, TimeSpan.Zero);

    }
}
