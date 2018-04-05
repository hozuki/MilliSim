using System;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Core;

namespace OpenMLTD.TheaterDays.Subsystems.Bvs {
    internal sealed class TDCommunication : DisposableBase {

        internal TDCommunication([NotNull] Theater theater) {
            Server = new TDSimulatorServer(this);
            Client = new TDSimulatorClient(this);
            Theater = theater;
        }

        [CanBeNull]
        internal Uri EditorServerUri { get; set; }

        [NotNull]
        internal TDSimulatorServer Server { get; }

        [NotNull]
        internal TDSimulatorClient Client { get; }

        [NotNull]
        internal Theater Theater { get; }

        protected override void Dispose(bool disposing) {
            Client.Dispose();
            Server.Dispose();
        }

    }
}
