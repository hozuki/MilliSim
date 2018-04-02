using System.Threading.Tasks;
using JetBrains.Annotations;
using OpenMLTD.Piyopiyo;
using OpenMLTD.Piyopiyo.Net.JsonRpc;
using OpenMLTD.TheaterDays.Subsystems.Bvs.Models;

namespace OpenMLTD.TheaterDays.Subsystems.Bvs {
    internal sealed class TDSimulatorClient : JsonRpcClient {

        internal TDSimulatorClient([NotNull] TDCommunication communication) {
            _communication = communication;
        }

        internal Task SendLaunchedNotification() {
            var serverUri = _communication.EditorServerUri;

            if (serverUri == null) {
                return Task.FromResult(0);
            }

            var simulatorServerPort = _communication.Server.EndPoint.Port;

            var param0Object = new GeneralSimLaunchedNotificationParams {
                SimulatorServerUri = $"http://localhost:{simulatorServerPort}/"
            };

            return SendNotificationAsync(serverUri, CommonProtocolMethodNames.General_SimLaunched, new[] { param0Object });
        }

        internal Task SendPlayingNotification() {
            return SendNotificationWithEmptyBody(CommonProtocolMethodNames.Preview_Playing);
        }

        internal Task SendTickNotification() {
            return SendNotificationWithEmptyBody(CommonProtocolMethodNames.Preview_Tick);
        }

        internal Task SendPausedNotification() {
            return SendNotificationWithEmptyBody(CommonProtocolMethodNames.Preview_Paused);
        }

        internal Task SendStoppedNotification() {
            return SendNotificationWithEmptyBody(CommonProtocolMethodNames.Preview_Stopped);
        }

        internal Task SendSimExitedNotification() {
            return SendNotificationWithEmptyBody(CommonProtocolMethodNames.General_SimExited);
        }

        internal Task SendReloadedNotification() {
            return SendNotificationWithEmptyBody(CommonProtocolMethodNames.Edit_Reloaded);
        }

        internal Task SendSoughtNotification() {
            return SendNotificationWithEmptyBody(CommonProtocolMethodNames.Preview_Sought);
        }

        private Task SendNotificationWithEmptyBody([NotNull] string methodName) {
            var serverUri = _communication.EditorServerUri;

            if (serverUri == null) {
                return Task.FromResult(0);
            }

            return SendNotificationAsync(serverUri, methodName);
        }

        private readonly TDCommunication _communication;

    }
}
