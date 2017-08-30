using System;
using System.Drawing;
using System.IO;
using System.Threading;
using JetBrains.Annotations;
using MimeTypes;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Rendering;
using OpenMLTD.MilliSim.Rendering.Drawing;
using OpenMLTD.MilliSim.Rendering.Extensions;
using SharpDX.DXGI;
using SharpDX.MediaFoundation;

namespace OpenMLTD.MilliSim.Theater.Elements {
    public sealed class BackgroundVideo : BackgroundBase {

        ~BackgroundVideo() {
            CloseFile();
        }

        public override string Name { get; set; } = "Background Video";

        public event EventHandler<VideoStateChangedEventArgs> VideoStateChanged;

        public void OpenFile([NotNull] string path) {
            if (_fileStream != null) {
                CloseFile();
            }
            _fileStream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            _fileDataStream = new ByteStream(_fileStream);
            var fullPath = Path.GetFullPath(path);
            var absoluteUri = new Uri(fullPath);
            _mediaEngineEx.SetSourceFromByteStream(_fileDataStream, absoluteUri.AbsoluteUri);

            var fileExtension = Path.GetExtension(fullPath);
            var mimeType = MimeTypeMap.GetMimeType(fileExtension);
            _mediaEngine.CanPlayType(mimeType, out var answer);
            _canPlay = answer != MediaEngineCanPlay.NotSupported;
            if (!_canPlay) {
                // If MediaEngine cannot play this file, set the event to Set state to avoid clients waiting infinitely.
                // (See BackgroundVideoElement)
                _readyToPlayEvent.Set();
            }
        }

        public void CloseFile() {
            if (_fileStream == null) {
                return;
            }
            _readyToPlayEvent.Reset();
            Stop();
            _mediaEngineEx.SetSourceFromByteStream(NullSource, string.Empty);
            _fileDataStream.Dispose();
            _fileStream.Dispose();
            _fileStream = null;
            _fileDataStream = null;
        }

        public void Play() {
            if (!_canPlay) {
                return;
            }
            _mediaEngine.Play();
            _isVideoPaused = false;
            _isVideoStopped = false;
        }

        public void Pause() {
            if (!_canPlay) {
                return;
            }
            _mediaEngine.Pause();
            _isVideoPaused = true;
            // Doesn't change the stopped status.
        }

        public void Stop() {
            if (!_canPlay) {
                return;
            }
            _mediaEngine.Pause();
            _mediaEngine.CurrentTime = 0;
            _isVideoStopped = true;
            _isVideoPaused = false;
        }

        public double Volume {
            get {
                if (!_canPlay) {
                    return 0;
                }
                return _mediaEngine.Volume;
            }
            set {
                if (!_canPlay) {
                    return;
                }
                _mediaEngine.Volume = value;
            }
        }

        public void PauseOnFirstFrame() {
            if (!_canPlay) {
                return;
            }
            _mediaEngine.CurrentTime = 0;
            Pause();
        }

        public void TogglePause() {
            if (!_canPlay) {
                return;
            }
            if (IsStopped) {
                return;
            }
            if (IsPaused) {
                Play();
            } else {
                Pause();
            }
        }

        public bool IsStopped => _isVideoStopped;

        public bool IsPaused => _isVideoPaused;

        public bool IsReadyToPlay => _readyToPlay;

        public bool CanPlay => _canPlay;

        public ManualResetEvent ReadyToPlayEvent => _readyToPlayEvent;

        protected override void OnDraw(GameTime gameTime, RenderContext context) {
            base.OnDraw(gameTime, context);

            // Resizing... is troublesome.

            var mediaEngine = _mediaEngine;

            if (_readyToPlay && !_isVideoStopped) {
                if (_videoSize == null) {
                    mediaEngine.GetNativeVideoSize(out var w, out var h);
                    _videoSize = new Size(w, h);
                }
                // Transfer frame if a new one is available
                if (mediaEngine.OnVideoStreamTick(out var _)) {
                    var videoRect = new SharpDX.Rectangle(0, 0, _videoSize.Value.Width, _videoSize.Value.Height);
                    mediaEngine.TransferVideoFrame(context.RenderTarget.BackBufferSurface, null, videoRect, null);
                }
            }
        }

        protected override void OnGotContext(RenderContext context) {
            MediaManager.Startup();

            var mediaEngineFactory = new MediaEngineClassFactory();
            _mediaEngineFactory = mediaEngineFactory;

            var mediaAttr = new MediaEngineAttributes();
            mediaAttr.VideoOutputFormat = (int)Format.B8G8R8A8_UNorm;
            mediaAttr.DxgiManager = context.DxgiDeviceManager;

            // Creates MediaEngine for AudioOnly 
            var mediaEngine = new MediaEngine(mediaEngineFactory, mediaAttr);
            _mediaEngine = mediaEngine;

            // Register our PlayBackEvent
            mediaEngine.PlaybackEvent += OnPlaybackCallback;

            // Query for MediaEngineEx interface
            var mediaEngineEx = mediaEngine.QueryInterface<MediaEngineEx>();
            _mediaEngineEx = mediaEngineEx;

            base.OnGotContext(context);
        }

        protected override void OnLostContext(RenderContext context) {
            CloseFile();

            _mediaEngine.Dispose();
            _mediaEngineFactory.Dispose();

            MediaManager.Shutdown();

            _readyToPlay = false;

            base.OnLostContext(context);
        }

        private void OnPlaybackCallback(MediaEngineEvent playEvent, long param1, int param2) {
            switch (playEvent) {
                case MediaEngineEvent.CanPlay:
                    _readyToPlay = true;
                    _readyToPlayEvent.Set();
                    break;
                case MediaEngineEvent.Pause:
                    _isVideoPaused = true;
                    break;
                case MediaEngineEvent.Play:
                case MediaEngineEvent.Playing:
                    _isVideoPaused = false;
                    break;
                case MediaEngineEvent.TimeUpdate:
                    break;
                case MediaEngineEvent.Error:
                case MediaEngineEvent.Abort:
                case MediaEngineEvent.Ended:
                    _isVideoStopped = true;
                    _isVideoPaused = false;
                    _isVideoEnded = true;
                    break;
            }
            VideoStateChanged?.Invoke(this, new VideoStateChangedEventArgs(playEvent));
        }

        private MediaEngineClassFactory _mediaEngineFactory;
        private MediaEngine _mediaEngine;
        private MediaEngineEx _mediaEngineEx;
        private ByteStream _fileDataStream;
        private FileStream _fileStream;

        // Is the video ready to play?
        private bool _readyToPlay;
        // No need to call its Dispose().
        private readonly ManualResetEvent _readyToPlayEvent = new ManualResetEvent(false);
        // Native size of the video.
        private Size? _videoSize;
        private bool _isVideoStopped;
        private bool _isVideoPaused;
        private bool _isVideoEnded;

        private bool _canPlay;

        private static readonly ByteStream NullSource = new ByteStream(new byte[0]);

    }
}
