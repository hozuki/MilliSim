using System;

namespace OpenMLTD.MilliSim.Extension.Contributed.Scores.StandardScoreFormats.StarlightDirector.Models {
    public static class StarlightID {

        public static Guid GetGuidFromInt32(int id) {
            return new Guid(id, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
        }

        public static readonly Guid Invalid = Guid.Empty;

    }
}
