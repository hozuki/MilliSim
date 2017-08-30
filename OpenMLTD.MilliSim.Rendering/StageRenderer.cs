using System;
using System.Collections.Generic;
using System.Drawing;
using OpenMLTD.MilliSim.Core;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.MediaFoundation;
using Color = System.Drawing.Color;
using Factory = SharpDX.DXGI.Factory;

namespace OpenMLTD.MilliSim.Rendering {
    public abstract class StageRenderer : DisposableBase {

        protected StageRenderer(Game game) {
            Game = game;
        }

        public Game Game { get; }

        public Color ClearColor { get; set; } = Color.Black;

        public abstract Size ClientSize { get; }

        public void Draw(IReadOnlyList<IDrawable> drawables, GameTime gameTime) {
            var context = _renderContext;

            lock (_sizeLock.NewReadLock()) {
                if (_isSizeChanged) {
                    foreach (var element in drawables) {
                        element.OnLostContext(context);
                    }

                    context?.Dispose();
                    _dxgiFactory?.Dispose();
                    _swapChain?.Dispose();
                    _direct3DDevice?.Dispose();
                    //_direct2DDevice?.Dispose();
                    //_direct2DContext?.Dispose();
                    _dxgiDeviceManager?.Dispose();

                    _swapChainDescription.ModeDescription.Width = _newSize.Width;
                    _swapChainDescription.ModeDescription.Height = _newSize.Height;

                    SharpDX.Direct3D11.Device.CreateWithSwapChain(DriverType.Hardware, D3DDeviceCreationFlags, _swapChainDescription, out _direct3DDevice, out _swapChain);
                    _dxgiDevice = _direct3DDevice.QueryInterface<SharpDX.DXGI.Device>();
                    _dxgiFactory = _swapChain.GetParent<Factory>();

                    // Video (EVR) initialization.
                    var multithread = _direct3DDevice.QueryInterface<DeviceMultithread>();
                    multithread.SetMultithreadProtected(true);
                    _dxgiDeviceManager = new DXGIDeviceManager();
                    _dxgiDeviceManager.ResetDevice(_direct3DDevice);

                    context = new RenderContext(this, new Size(_newSize.Width, _newSize.Height));
                    _renderContext = context;

                    foreach (var drawable in drawables) {
                        drawable.OnGotContext(context);
                    }

                    _isSizeChanged = false;

                    ++_controlResizeCounter;
                    if (_controlResizeCounter == StartupControlResizeCount) {
                        // WTF...
                        // Look into MediaEngine and DXGIDeviceManager?
                        Game.Window.Invoke(new Action(() => Game.Window.RaiseStageReady(EventArgs.Empty)));

                        foreach (var element in drawables) {
                            element.OnStageReady(context);
                        }
                    }
                }
            }

            context.Clear();

            foreach (var drawable in drawables) {
                drawable.Draw(gameTime, context);
            }

            context.Present();
        }

        public RenderContext RenderContext => _renderContext;

        internal SharpDX.Direct3D11.Device Direct3DDevice => _direct3DDevice;

        internal SharpDX.DXGI.Device DxgiDevice => _dxgiDevice;

        internal Factory DxgiFactory => _dxgiFactory;

        internal SharpDX.DirectWrite.Factory DirectWriteFactory => _directWriteFactory;

        internal SwapChain SwapChain => _swapChain;

        internal SwapChainDescription SwapChainDescription => _swapChainDescription;

        internal DXGIDeviceManager DxgiDeviceManager => _dxgiDeviceManager;

        protected override void Dispose(bool disposing) {
            if (!disposing) {
                return;
            }
            _directWriteFactory.Dispose();
            _dxgiFactory.Dispose();
            _swapChain.Dispose();
            _direct3DDevice.Dispose();
            _dxgiDeviceManager.Dispose();
        }

        protected abstract void CreateSwapChainAndDevice(out SwapChainDescription swapChainDescription, out SwapChain swapChain, out SharpDX.Direct3D11.Device device);

        protected virtual void OnAfterInitialization() {
        }

        // Called in child classes' constructors.
        protected void Initialize() {
            CreateSwapChainAndDevice(out _swapChainDescription, out _swapChain, out _direct3DDevice);

            _dxgiDevice = _direct3DDevice.QueryInterface<SharpDX.DXGI.Device>();
            _dxgiFactory = _swapChain.GetParent<Factory>();

            // Direct2D initialization.
            _directWriteFactory = new SharpDX.DirectWrite.Factory();

            // Video (EVR) initialization.
            var multithread = _direct3DDevice.QueryInterface<DeviceMultithread>();
            multithread.SetMultithreadProtected(true);
            _dxgiDeviceManager = new DXGIDeviceManager();
            _dxgiDeviceManager.ResetDevice(_direct3DDevice);

            var context = new RenderContext(this, ClientSize);
            _renderContext = context;
            foreach (var element in Game.Elements) {
                (element as IDrawable)?.OnGotContext(context);
            }

            OnAfterInitialization();
        }

        protected bool _isSizeChanged;
        protected Size2 _newSize;
        protected readonly SimpleUsingLock _sizeLock = new SimpleUsingLock();

        // For D2D interop, and video output.
        protected const DeviceCreationFlags D3DDeviceCreationFlags = DeviceCreationFlags.BgraSupport | DeviceCreationFlags.VideoSupport;

        private SharpDX.Direct3D11.Device _direct3DDevice;
        private SharpDX.DXGI.Device _dxgiDevice;
        private SwapChainDescription _swapChainDescription;
        private SwapChain _swapChain;
        private Factory _dxgiFactory;
        private SharpDX.DirectWrite.Factory _directWriteFactory;
        private DXGIDeviceManager _dxgiDeviceManager;

        protected static readonly Rational DefaultRefreshRate = new Rational(60, 1);

        private RenderContext _renderContext;

        // These two counters work as follows.
        // We assume that the stage is resized only for a certain number of times. During each size change,
        // increase the counter. And finally, all resizing actions are complete and the size will never
        // change in the future.
        // In current model, the size changes 1 time. One for window creation (by WinForms), one for setting
        // ClientSize on GameWindow (by user's config entry).
        private int _controlResizeCounter;
        private static int StartupControlResizeCount = 1;

    }
}
