using OpenMLTD.MilliSim.Core.Entities.Runtime;
using OpenMLTD.MilliSim.Core.Entities.Source;

namespace OpenMLTD.MilliSim.Core.Entities.Extending {

    public delegate RuntimeNote[] NotesCreation(SourceNote note, Conductor[] conductors, SourceNote[] gameNotes, ref int currentID);

}
