using System;
using System.Drawing;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Graphics.Extensions;
using OpenMLTD.MilliSim.Graphics.Rendering;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.MediaFoundation;
using Color = System.Drawing.Color;
using Factory = SharpDX.DXGI.Factory;

namespace OpenMLTD.MilliSim.Graphics {
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

            FrequentlyUsedStates = new FrequentlyUsedStates(this);

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

        public FrequentlyUsedStates FrequentlyUsedStates { get; }

        private SwapChainDescription SwapChainDescription { get; }

        public void ClearAll() {
            ClearAll(RenderTarget);
        }

        public void ClearAll(Color clearColor) {
            ClearAll(RenderTarget, clearColor);
        }

        public void ClearAll(RenderTarget target) {
            ClearAll(target, Renderer.ClearColor);
        }

        public void ClearAll(RenderTarget target, Color clearColor) {
            var immediateContext = Direct3DDevice.ImmediateContext;
            immediateContext.ClearDepthStencilView(target.DepthView, DepthStencilClearFlags.Depth, 1.0f, 0);
            immediateContext.ClearRenderTargetView(target.RenderTargetView, clearColor.ToRC4());
        }

        public void Clear2D() {
            Clear2D(RenderTarget);
        }

        public void Clear2D(Color clearColor) {
            Clear2D(RenderTarget, clearColor);
        }

        public void Clear2D(RenderTarget target) {
            Clear2D(target, Renderer.ClearColor);
        }

        public void Clear2D(RenderTarget target, Color clearColor) {
            target.DeviceContext2D.Clear(clearColor.ToRC4());
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

        /// <summary>
        /// Sets the input layout and primitive topology, then starts a new 3D drawing pass using the default state settings (<see cref="OpenMLTD.MilliSim.Graphics.Rendering.FrequentlyUsedStates.AlphaBlend"/>, <see cref="OpenMLTD.MilliSim.Graphics.Rendering.FrequentlyUsedStates.LessEqual"/>, <see cref="OpenMLTD.MilliSim.Graphics.Rendering.FrequentlyUsedStates.NoCull"/>).
        /// </summary>
        /// <param name="inputLayout">The input layout indicating how vertices are stored.</param>
        /// <param name="topology">The primitive topology indicating how indices are stored.</param>
        public void Begin3D(InputLayout inputLayout, PrimitiveTopology topology) {
            var states = FrequentlyUsedStates;
            Begin3D(inputLayout, topology, states.AlphaBlend, states.LessEqual, states.NoCull);
        }

        /// <summary>
        /// Sets the input layout and primitive topology, then starts a new 3D drawing pass using custom state settings.
        /// </summary>
        /// <param name="inputLayout">The input layout indicating how vertices are stored.</param>
        /// <param name="topology">The primitive topology indicating how indices are stored.</param>
        /// <param name="blend">The alpha blend state.</param>
        /// <param name="depthStencil">The depth-stencil comparer state.</param>
        /// <param name="rasterizer">The rasterizer state.</param>
        public void Begin3D(InputLayout inputLayout, PrimitiveTopology topology, BlendState blend, DepthStencilState depthStencil, RasterizerState rasterizer) {
            var ctx = Direct3DDevice.ImmediateContext;
            ctx.InputAssembler.InputLayout = inputLayout;
            ctx.InputAssembler.PrimitiveTopology = topology;
            ctx.Rasterizer.State = rasterizer;
            ctx.OutputMerger.BlendState = blend;
            ctx.OutputMerger.DepthStencilState = depthStencil;
        }

        /// <summary>
        /// Sets the input layout and primitive topology, then starts a new 3D drawing pass immediately.
        /// </summary>
        /// <param name="inputLayout">The input layout indicating how vertices are stored.</param>
        /// <param name="topology">The primitive topology indicating how indices are stored.</param>
        /// <remarks>All other states of the renderer are stay unchanged. If you use this method, you must specify the states in effects file (.fx).</remarks>
        public void Begin3DFast(InputLayout inputLayout, PrimitiveTopology topology) {
            var ctx = Direct3DDevice.ImmediateContext;
            ctx.InputAssembler.InputLayout = inputLayout;
            ctx.InputAssembler.PrimitiveTopology = topology;
        }

        /// <summary>
        /// Ends current 3D drawing pass.
        /// </summary>
        public void End3D() {
            var ctx = Direct3DDevice.ImmediateContext;
            ctx.InputAssembler.InputLayout = null;
            ctx.Rasterizer.State = null;
            ctx.OutputMerger.BlendState = null;
            ctx.OutputMerger.DepthStencilState = null;
        }

        public void Begin2D() {
            RenderTarget.DeviceContext2D.BeginDraw();
        }

        public void End2D() {
            RenderTarget.DeviceContext2D.EndDraw();
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
            FrequentlyUsedStates.Dispose();
            _rootRenderTarget.Dispose();
        }

        private RenderTarget _currentRenderTarget;
        private RenderTarget _rootRenderTarget;
        private Viewport _viewport;

    }
}
