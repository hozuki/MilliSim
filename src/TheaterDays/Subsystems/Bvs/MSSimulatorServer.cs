using System.Diagnostics;
using System.Net;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Extension.Components.ScoreComponents;
using OpenMLTD.MilliSim.Foundation.Extensions;
using OpenMLTD.Piyopiyo;
using OpenMLTD.Piyopiyo.Extensions;
using OpenMLTD.Piyopiyo.Net;
using OpenMLTD.Piyopiyo.Simulator;
using OpenMLTD.TheaterDays.Subsystems.Bvs.Models;

namespace OpenMLTD.TheaterDays.Subsystems.Bvs {
    internal sealed class MSSimulatorServer : SimulatorServer {

        internal MSSimulatorServer([NotNull] MSCommunication communication) {
            _communication = communication;
        }

        protected override void OnGeneralEdExit(object sender, JsonRpcMethodEventArgs e) {
            if (JsonRpcHelper.IsRequestValid(e.ParsedRequestObject, out string errorMessage)) {
                e.Context.RpcOk();
            } else {
                Debug.Print(errorMessage);
                e.Context.RpcError(JsonRpcErrorCodes.InvalidRequest, errorMessage);
            }
        }

        protected override void OnGeneralSimInitialize(object sender, JsonRpcMethodEventArgs e) {
            if (JsonRpcHelper.IsRequestValid(e.ParsedRequestObject, out string errorMessage)) {
                e.Context.RpcOk();
            } else {
                Debug.Print(errorMessage);
                e.Context.RpcError(JsonRpcErrorCodes.InvalidRequest, errorMessage);
            }
        }

        protected override void OnGeneralSimExit(object sender, JsonRpcMethodEventArgs e) {
            if (JsonRpcHelper.IsRequestValid(e.ParsedRequestObject, out string errorMessage)) {
                e.Context.RpcOk();
            } else {
                Debug.Print(errorMessage);
                e.Context.RpcError(JsonRpcErrorCodes.InvalidRequest, errorMessage);
            }
        }

        protected override void OnGeneralPreviewPlay(object sender, JsonRpcMethodEventArgs e) {
            if (JsonRpcHelper.IsRequestValid(e.ParsedRequestObject, out string errorMessage)) {
                e.Context.RpcOk();
            } else {
                Debug.Print(errorMessage);
                e.Context.RpcError(JsonRpcErrorCodes.InvalidRequest, errorMessage);
            }
        }

        protected override void OnGeneralPreviewPause(object sender, JsonRpcMethodEventArgs e) {
            if (JsonRpcHelper.IsRequestValid(e.ParsedRequestObject, out string errorMessage)) {
                e.Context.RpcOk();
            } else {
                Debug.Print(errorMessage);
                e.Context.RpcError(JsonRpcErrorCodes.InvalidRequest, errorMessage);
            }
        }

        protected override void OnGeneralPreviewStop(object sender, JsonRpcMethodEventArgs e) {
            if (JsonRpcHelper.IsRequestValid(e.ParsedRequestObject, out string errorMessage)) {
                e.Context.RpcOk();
            } else {
                Debug.Print(errorMessage);
                e.Context.RpcError(JsonRpcErrorCodes.InvalidRequest, errorMessage);
            }
        }

        protected override void OnGeneralPreviewGetPlaybackState(object sender, JsonRpcMethodEventArgs e) {
            if (JsonRpcHelper.IsRequestValid(e.ParsedRequestObject, out string errorMessage)) {
                e.Context.RpcOk();
            } else {
                Debug.Print(errorMessage);
                e.Context.RpcError(JsonRpcErrorCodes.InvalidRequest, errorMessage);
            }
        }

        protected override void OnGeneralPreviewGotoTime(object sender, JsonRpcMethodEventArgs e) {
            if (JsonRpcHelper.IsRequestValid(e.ParsedRequestObject, out string errorMessage)) {
                e.Context.RpcOk();
            } else {
                Debug.Print(errorMessage);
                e.Context.RpcError(JsonRpcErrorCodes.InvalidRequest, errorMessage);
            }
        }

        protected override void OnEditReload(object sender, JsonRpcMethodEventArgs e) {
            if (!JsonRpcHelper.IsRequestValid(e.ParsedRequestObject, out string errorMessage)) {
                Debug.Print(errorMessage);
                e.Context.RpcError(JsonRpcErrorCodes.InvalidRequest, errorMessage);

                return;
            }

            var scoreLoader = _communication.Theater.FindSingleElement<ScoreLoader>();

            if (scoreLoader == null) {
                e.Context.RpcError(JsonRpcErrorCodes.InternalError, "Cannot find the ScoreLoader.", statusCode: HttpStatusCode.InternalServerError);
                return;
            }

            var requestObject = JsonRpcHelper.TranslateAsRequest(e.ParsedRequestObject);
            var @params = JsonRpcRequestWrapper.ParamArrayToObject<EditReloadParams>(requestObject.Params);

            scoreLoader.LoadScoreFile(@params.ScoreFile, @params.ScoreIndex, @params.ScoreOffset);

            e.Context.RpcOk();

            Debug.Assert(_communication.EditorServerUri != null, "_communication.EditorServerUri != null");

#pragma warning disable 4014
            _communication.Client.CallAsync(_communication.EditorServerUri, CommonProtocolMethodNames.Edit_Reloaded);
#pragma warning restore 4014
        }

        private readonly MSCommunication _communication;

    }
}
