using System;
using System.Drawing;
using System.IO;
using System.Threading;
using JetBrains.Annotations;
using MimeTypes;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Graphics;
using SharpDX.DXGI;
using SharpDX.MediaFoundation;

namespace OpenMLTD.MilliSim.Theater.Elements {
    public sealed class BackgroundVideo : BackgroundBase {

        public BackgroundVideo(GameBase game)
            : base(game) {
        }

        ~BackgroundVideo() {
            CloseFile();
        }

        public override string Name { get; set; } = "Background Video";

        public event EventHandler<VideoStateChangedEventArgs> VideoStateChanged;

        public TimeSpan CurrentTime => _mediaEngine == null ? TimeSpan.Zero : TimeSpan.FromSeconds(_mediaEngine.CurrentTime);

        public void OpenFile([NotNull] string path) {
            if (_fileStream != null) {
                CloseFile();
            }

            var fullPath = Path.GetFullPath(path);
            _fileStream = File.Open(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            _fileDataStream = new ByteStream(_fileStream);

            var absoluteUri = new Uri(fullPath);
            _mediaEngineEx.SetSourceFromByteStream(_fileDataStream, absoluteUri.AbsoluteUri);

            var fileExtension = Path.GetExtension(fullPath);
            var mimeType = MimeTypeMap.GetMimeType(fileExtension);
            _mediaEngine.CanPlayType(mimeType, out var answer);
            CanPlay = answer != MediaEngineCanPlay.NotSupported;
            if (!CanPlay) {
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
            IsReadyToPlay = false;
            Stop();
            _mediaEngineEx.SetSourceFromByteStream(NullSource, string.Empty);
            _fileDataStream?.Dispose();
            _fileStream.Dispose();
            _fileStream = null;
            _fileDataStream = null;
        }

        public void Play() {
            if (!CanPlay) {
                return;
            }
            _mediaEngine.Play();
            IsPaused = false;
            IsStopped = false;
        }

        public void Pause() {
            if (!CanPlay) {
                return;
            }
            _mediaEngine.Pause();
            IsPaused = true;
            // Doesn't change the stopped status.
        }

        public void Stop() {
            if (!CanPlay) {
                return;
            }
            _mediaEngine.Pause();
            _mediaEngine.CurrentTime = 0;
            IsStopped = true;
            IsPaused = false;
        }

        public double Volume {
            get {
                if (!CanPlay) {
                    return 0;
                }
                return _mediaEngine.Volume;
            }
            set {
                if (!CanPlay) {
                    return;
                }
                _mediaEngine.Volume = value;
            }
        }

        public void PauseOnFirstFrame() {
            if (!CanPlay) {
                return;
            }
            // Force changing 'stopped' status.
            IsStopped = false;
            Pause();
            _mediaEngine.CurrentTime = 0;
        }

        public void TogglePause() {
            if (!CanPlay) {
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

        public void WaitUntilReady() {
            _readyToPlayEvent.WaitOne();
        }

        public MediaError GetError() {
            return _mediaEngine.Error;
        }

        public bool IsStopped { get; private set; }

        public bool IsPaused { get; private set; }

        public bool CanPlay { get; private set; }

        public bool IsEnded { get; private set; }

        // TODO: when Visible=false, this method will not be called, and the video's audio playback suspends.
        protected override void OnDraw(GameTime gameTime, RenderContext context) {
            base.OnDraw(gameTime, context);

            // Resizing... is troublesome.

            var mediaEngine = _mediaEngine;

            if (IsReadyToPlay && !IsStopped) {
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

            IsReadyToPlay = false;

            base.OnLostContext(context);
        }

        protected override void OnStageReady(RenderContext context) {
            base.OnStageReady(context);
            _isStageReady = true;
        }

        private bool IsReadyToPlay { get; set; }

        private void OnPlaybackCallback(MediaEngineEvent playEvent, long param1, int param2) {
            var settings = Program.Settings;

            switch (playEvent) {
                case MediaEngineEvent.CanPlay:
                    if (_isStageReady) {
                        _readyToPlayEvent.Set();
                        IsReadyToPlay = true;
                        IsEnded = false;
                        IsStopped = true;
                    }
                    Volume = settings.Media.BackgroundAnimationVolume.Value;
                    break;
                case MediaEngineEvent.Pause:
                    IsPaused = true;
                    break;
                case MediaEngineEvent.Play:
                case MediaEngineEvent.Playing:
                    IsPaused = false;
                    break;
                case MediaEngineEvent.TimeUpdate:
                    break;
                case MediaEngineEvent.Ended:
                    IsStopped = true;
                    IsPaused = false;
                    IsEnded = true;
                    break;
                case MediaEngineEvent.Error:
                case MediaEngineEvent.Abort:
                    IsStopped = true;
                    IsPaused = true;
                    IsEnded = true;
                    _readyToPlayEvent.Reset();
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
        // No need to call its Dispose().
        // Native size of the video.
        private Size? _videoSize;

        private bool _isStageReady;

        // Warning: Not disposed...
        private readonly ManualResetEvent _readyToPlayEvent = new ManualResetEvent(false);

        private static readonly ByteStream NullSource = new ByteStream(new byte[0]);

    }
}
