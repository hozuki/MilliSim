using JetBrains.Annotations;
using OpenMLTD.Piyopiyo.Net;

namespace OpenMLTD.TheaterDays.Subsystems.Bvs {
    internal sealed class MSSimulatorClient : JsonRpcClient {

        internal MSSimulatorClient([NotNull] MSCommunication communication) {
            _communication = communication;
        }

        private readonly MSCommunication _communication;

    }
}
