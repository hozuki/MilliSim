using OpenMLTD.MilliSim.Contributed.Scores.Runtime;
using OpenMLTD.MilliSim.Contributed.Scores.Source;

namespace OpenMLTD.MilliSim.Contributed.Scores {

    public delegate RuntimeNote[] NotesCreation(SourceNote note, Conductor[] conductors, SourceNote[] gameNotes, ref int currentID);

}
