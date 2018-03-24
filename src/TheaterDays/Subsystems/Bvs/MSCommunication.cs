using System;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Core;

namespace OpenMLTD.TheaterDays.Subsystems.Bvs {
    internal sealed class MSCommunication : DisposableBase {

        internal MSCommunication([NotNull] Theater theater) {
            _server = new MSSimulatorServer(this);
            _client = new MSSimulatorClient(this);
            _theater = theater;
        }

        [CanBeNull]
        internal Uri EditorServerUri { get; set; }

        [NotNull]
        internal MSSimulatorServer Server => _server;

        [NotNull]
        internal MSSimulatorClient Client => _client;

        [NotNull]
        internal Theater Theater => _theater;

        protected override void Dispose(bool disposing) {
            _client.Dispose();
            _server.Dispose();
        }

        private readonly Theater _theater;
        private readonly MSSimulatorServer _server;
        private readonly MSSimulatorClient _client;

    }
}
