using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Extension.Components.CoreComponents;
using OpenMLTD.MilliSim.Extension.Components.ScoreComponents;
using OpenMLTD.MilliSim.Foundation.Extensions;
using OpenMLTD.Piyopiyo.Extensions;
using OpenMLTD.Piyopiyo.Net;
using OpenMLTD.Piyopiyo.Simulator;
using OpenMLTD.TheaterDays.Subsystems.Bvs.Models;
using OpenMLTD.TheaterDays.Subsystems.Bvs.Models.Proposals;

namespace OpenMLTD.TheaterDays.Subsystems.Bvs {
    internal sealed class TDSimulatorServer : SimulatorServer {

        internal TDSimulatorServer([NotNull] TDCommunication communication) {
            _communication = communication;
        }

        protected override void OnGeneralEdExit(object sender, JsonRpcMethodEventArgs e) {
            LogToScreen("Received request: general/edExit");

            if (JsonRpcHelper.IsRequestValid(e.ParsedRequestObject, out string errorMessage)) {
                e.Context.RpcOk();

                _communication.EditorServerUri = null;
            } else {
                Debug.Print(errorMessage);
                e.Context.RpcError(JsonRpcErrorCodes.InvalidRequest, errorMessage);
            }
        }

        protected override void OnGeneralSimInitialize(object sender, JsonRpcMethodEventArgs e) {
            LogToScreen("Received request: general/simInitialize");

            if (JsonRpcHelper.IsRequestValid(e.ParsedRequestObject, out string errorMessage)) {
                var requestObject = JsonRpcHelper.TranslateAsRequest(e.ParsedRequestObject);
                var @params = JsonRpcRequestWrapper.ParamArrayToObject<GeneralSimInitializeRequestParams>(requestObject.Params);

                var selectedFormat = SelectFormat(@params.Data.SupportedFormats);

                var responseResult = new GeneralSimInitializeResponseResult {
                    SelectedFormat = selectedFormat
                };

                e.Context.RpcOk(responseResult);

                _communication.Client.SendInitializedNotification();
            } else {
                Debug.Print(errorMessage);
                e.Context.RpcError(JsonRpcErrorCodes.InvalidRequest, errorMessage);
            }
        }

        protected override void OnGeneralSimExit(object sender, JsonRpcMethodEventArgs e) {
            LogToScreen("Received request: general/simExit");

            if (JsonRpcHelper.IsRequestValid(e.ParsedRequestObject, out string errorMessage)) {
                e.Context.RpcOk();

                _communication.Client.SendExitedNotification();

                _communication.Theater.Exit();
            } else {
                Debug.Print(errorMessage);
                e.Context.RpcError(JsonRpcErrorCodes.InvalidRequest, errorMessage);
            }
        }

        protected override void OnGeneralPreviewPlay(object sender, JsonRpcMethodEventArgs e) {
            LogToScreen("Received request: preview/play");

            if (JsonRpcHelper.IsRequestValid(e.ParsedRequestObject, out string errorMessage)) {
                var syncTimer = _communication.Theater.FindSingleElement<SyncTimer>();

                if (syncTimer == null) {
                    e.Context.RpcError(JsonRpcErrorCodes.InternalError, "Cannot find the " + nameof(SyncTimer) + " component.", statusCode: HttpStatusCode.InternalServerError);
                    return;
                }

                syncTimer.Start();

                e.Context.RpcOk();

                _communication.Client.SendPlayingNotification();
            } else {
                Debug.Print(errorMessage);
                e.Context.RpcError(JsonRpcErrorCodes.InvalidRequest, errorMessage);
            }
        }

        protected override void OnGeneralPreviewPause(object sender, JsonRpcMethodEventArgs e) {
            LogToScreen("Received request: preview/pause");

            if (JsonRpcHelper.IsRequestValid(e.ParsedRequestObject, out string errorMessage)) {
                var syncTimer = _communication.Theater.FindSingleElement<SyncTimer>();

                if (syncTimer == null) {
                    e.Context.RpcError(JsonRpcErrorCodes.InternalError, "Cannot find the " + nameof(SyncTimer) + " component.", statusCode: HttpStatusCode.InternalServerError);
                    return;
                }

                syncTimer.Pause();

                e.Context.RpcOk();

                _communication.Client.SendPausedNotification();
            } else {
                Debug.Print(errorMessage);
                e.Context.RpcError(JsonRpcErrorCodes.InvalidRequest, errorMessage);
            }
        }

        protected override void OnGeneralPreviewStop(object sender, JsonRpcMethodEventArgs e) {
            LogToScreen("Received request: preview/stop");

            if (JsonRpcHelper.IsRequestValid(e.ParsedRequestObject, out string errorMessage)) {
                var syncTimer = _communication.Theater.FindSingleElement<SyncTimer>();

                if (syncTimer == null) {
                    e.Context.RpcError(JsonRpcErrorCodes.InternalError, "Cannot find the " + nameof(SyncTimer) + " component.", statusCode: HttpStatusCode.InternalServerError);
                    return;
                }

                syncTimer.Stop();

                e.Context.RpcOk();

                _communication.Client.SendStoppedNotification();
            } else {
                Debug.Print(errorMessage);
                e.Context.RpcError(JsonRpcErrorCodes.InvalidRequest, errorMessage);
            }
        }

        protected override void OnGeneralPreviewGetPlaybackState(object sender, JsonRpcMethodEventArgs e) {
            LogToScreen("Received request: preview/getPlaybackState");

            if (JsonRpcHelper.IsRequestValid(e.ParsedRequestObject, out string errorMessage)) {
                e.Context.RpcOk();
            } else {
                Debug.Print(errorMessage);
                e.Context.RpcError(JsonRpcErrorCodes.InvalidRequest, errorMessage);
            }
        }

        protected override void OnGeneralPreviewGotoTime(object sender, JsonRpcMethodEventArgs e) {
            LogToScreen("Received request: preview/gotoTime");

            e.Context.RpcErrorNotImplemented();
            //if (JsonRpcHelper.IsRequestValid(e.ParsedRequestObject, out string errorMessage)) {
            //    e.Context.RpcOk();

            //    _communication.Client.SendSeekingCompletedNotification();
            //} else {
            //    Debug.Print(errorMessage);
            //    e.Context.RpcError(JsonRpcErrorCodes.InvalidRequest, errorMessage);
            //}
        }

        protected override void OnEditReload(object sender, JsonRpcMethodEventArgs e) {
            LogToScreen("Received request: edit/reload");

            if (!JsonRpcHelper.IsRequestValid(e.ParsedRequestObject, out string errorMessage)) {
                Debug.Print(errorMessage);
                e.Context.RpcError(JsonRpcErrorCodes.InvalidRequest, errorMessage);

                return;
            }

            var scoreLoader = _communication.Theater.FindSingleElement<ScoreLoader>();

            if (scoreLoader == null) {
                e.Context.RpcError(JsonRpcErrorCodes.InternalError, "Cannot find the " + nameof(ScoreLoader) + " component.", statusCode: HttpStatusCode.InternalServerError);
                return;
            }

            var requestObject = JsonRpcHelper.TranslateAsRequest(e.ParsedRequestObject);
            var @params = JsonRpcRequestWrapper.ParamArrayToObject<EditReloadRequestParams>(requestObject.Params);

            if (!string.IsNullOrEmpty(@params.ScoreFile)) {
                if (File.Exists(@params.ScoreFile)) {
                    var backgroundMusic = _communication.Theater.FindSingleElement<BackgroundMusic>();

                    if (!string.IsNullOrEmpty(@params.BackgroundMusic)) {
                        if (backgroundMusic == null) {
                            e.Context.RpcError(JsonRpcErrorCodes.InternalError, "Cannot find the " + nameof(BackgroundMusic) + " component.", statusCode: HttpStatusCode.InternalServerError);
                            return;
                        }

                        backgroundMusic.LoadMusic(@params.BackgroundMusic);
                    }

                    scoreLoader.LoadScoreFile(@params.ScoreFile, @params.ScoreIndex, @params.ScoreOffset);
                } else {
                    LogToScreen($"Not found: {@params.ScoreFile}");
                }
            }

            e.Context.RpcOk();

            // DO NOT await
            _communication.Client.SendReloadedNotification();
        }

        private void LogToScreen([NotNull] string message) {
            var debug = _communication.Theater.FindSingleElement<DebugOverlay>();

            if (debug != null) {
                message = "[SimServer] " + message;
                debug.AddLine(message);
            }
        }

        [CanBeNull]
        private SelectedFormatDescriptor SelectFormat([NotNull, ItemNotNull] SupportedFormatDescriptor[] supportedFormats) {
            SelectedFormatDescriptor selectedFormat = null;

            foreach (var format in supportedFormats) {
                var locals = _supportedScoreFileFormats.Where(f => f.Game == format.Game && f.FormatId == format.FormatId);

                foreach (var local in locals) {
                    if (Array.IndexOf(format.Versions, local.Version) >= 0) {
                        selectedFormat = local;
                        break;
                    }
                }
            }

            return selectedFormat;
        }

        private readonly SelectedFormatDescriptor[] _supportedScoreFileFormats = {
            new SelectedFormatDescriptor {Game = "mirishita", FormatId = "unity3d", Version = "*"},
            new SelectedFormatDescriptor {Game = "deresute", FormatId = "sldproj", Version = "400"},
            new SelectedFormatDescriptor {Game = "deresute", FormatId = "sldproj", Version = "301"},
            new SelectedFormatDescriptor {Game = "deresute", FormatId = "sldproj", Version = "300"},
            new SelectedFormatDescriptor {Game = "deresute", FormatId = "sldproj", Version = "200"},
        };

        private readonly TDCommunication _communication;

    }
}
