using System.Drawing;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Rendering.Extensions;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.MediaFoundation;
using Color = System.Drawing.Color;
using Factory = SharpDX.DXGI.Factory;

namespace OpenMLTD.MilliSim.Rendering {
    public sealed class RenderContext : DisposableBase {

        internal RenderContext(StageRenderer renderer, Size clientSize) {
            Renderer = renderer;
            ClientSize = clientSize;

            Direct3DDevice = renderer.Direct3DDevice;
            DxgiDevice = renderer.DxgiDevice;
            DxgiFactory = renderer.DxgiFactory;
            SwapChain = renderer.SwapChain;
            SwapChainDescription = renderer.SwapChainDescription;
            DirectWriteFactory = renderer.DirectWriteFactory;
            DxgiDeviceManager = renderer.DxgiDeviceManager;

            Initialize();
        }

        public StageRenderer Renderer { get; }

        public Size ClientSize { get; }

        public DXGIDeviceManager DxgiDeviceManager { get; }

        public SharpDX.Direct3D11.Device Direct3DDevice { get; }

        public SharpDX.DXGI.Device DxgiDevice { get; }

        public Factory DxgiFactory { get; }

        public SharpDX.DirectWrite.Factory DirectWriteFactory { get; }

        public SwapChain SwapChain { get; }

        private SwapChainDescription SwapChainDescription { get; }

        public void Clear() {
            Clear(RenderTarget);
        }

        public void Clear(Color clearColor) {
            Clear(RenderTarget, Renderer.ClearColor);
        }

        public void Clear(RenderTarget target) {
            Clear(target, Renderer.ClearColor);
        }

        public void Clear(RenderTarget target, Color clearColor) {
            var immediateContext = Direct3DDevice.ImmediateContext;
            immediateContext.ClearDepthStencilView(target.DepthView, DepthStencilClearFlags.Depth, 1.0f, 0);
            immediateContext.ClearRenderTargetView(target.RenderTargetView, clearColor.ToRC4());
        }

        public void SetRenderTarget([CanBeNull] RenderTarget target) {
            if (target == null) {
                target = _rootRenderTarget;
            }
            _currentRenderTarget = target;
            var outputMerger = Direct3DDevice.ImmediateContext.OutputMerger;
            outputMerger.ResetTargets();
            outputMerger.SetRenderTargets(target.DepthView, target.RenderTargetView);
        }

        public RenderTarget RenderTarget => _currentRenderTarget;

        public RenderTarget CreateRenderTarget() {
            return new RenderTarget(this, false);
        }

        public void Begin3D() {
        }

        public void End3D() {
        }

        public void Begin2D() {
            RenderTarget.DeviceContext.BeginDraw();
        }

        public void End2D() {
            RenderTarget.DeviceContext.EndDraw();
        }

        public void Present() {
            // Update on each V-Blank.
            SwapChain.Present(1, PresentFlags.None);
        }

        private void Initialize() {
            var clientSize = ClientSize;
            var swapChain = SwapChain;
            var swapChainDescription = SwapChainDescription;

            swapChain.ResizeBuffers(swapChainDescription.BufferCount, clientSize.Width, clientSize.Height, Format.Unknown, SwapChainFlags.None);

            _rootRenderTarget = new RenderTarget(this, true);

            // Setup targets and viewport for rendering.
            _viewport = new Viewport(0, 0, clientSize.Width, clientSize.Height, 0.0f, 1.0f);
            Direct3DDevice.ImmediateContext.Rasterizer.SetViewport(_viewport);

            SetRenderTarget(null);
        }

        protected override void Dispose(bool disposing) {
            if (!disposing) {
                return;
            }
            _rootRenderTarget.Dispose();
        }

        private RenderTarget _currentRenderTarget;
        private RenderTarget _rootRenderTarget;
        private Viewport _viewport;

    }
}
