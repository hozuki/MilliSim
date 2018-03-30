using System;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Core;

namespace OpenMLTD.TheaterDays.Subsystems.Bvs {
    internal sealed class TDCommunication : DisposableBase {

        internal TDCommunication([NotNull] Theater theater) {
            _server = new TDSimulatorServer(this);
            _client = new TDSimulatorClient(this);
            Theater = theater;
        }

        [CanBeNull]
        internal Uri EditorServerUri { get; set; }

        [NotNull]
        internal TDSimulatorServer Server => _server;

        [NotNull]
        internal TDSimulatorClient Client => _client;

        [NotNull]
        internal Theater Theater { get; }

        protected override void Dispose(bool disposing) {
            _client.Dispose();
            _server.Dispose();
        }

        private readonly TDSimulatorServer _server;
        private readonly TDSimulatorClient _client;

    }
}
