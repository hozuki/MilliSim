using System.Collections.Generic;
using OpenMLTD.MilliSim.Core;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Direct3D11;
using SharpDX.DXGI;

namespace OpenMLTD.MilliSim.Rendering {
    public sealed class RenderTarget : DisposableBase {

        internal RenderTarget(RenderContext context, bool isRoot) {
            if (isRoot) {
                _backBuffer = context.SwapChain.GetBackBuffer<Texture2D>(0);
            } else {
                _backBuffer = new Texture2D(context.Direct3DDevice, new Texture2DDescription {
                    Format = Format.B8G8R8A8_UNorm,
                    ArraySize = 1,
                    MipLevels = 1,
                    Width = context.ClientSize.Width,
                    Height = context.ClientSize.Height,
                    SampleDescription = new SampleDescription(1, 0),
                    Usage = ResourceUsage.Default,
                    BindFlags = BindFlags.RenderTarget,
                    CpuAccessFlags = CpuAccessFlags.None,
                    OptionFlags = ResourceOptionFlags.Shared
                });
            }

            _backBufferSurface = _backBuffer.QueryInterface<Surface>();

            _renderView = new RenderTargetView(context.Direct3DDevice, _backBuffer);

            // Create the depth buffer
            _depthBuffer = new Texture2D(context.Direct3DDevice, new Texture2DDescription {
                Format = Format.D32_Float_S8X24_UInt,
                ArraySize = 1,
                MipLevels = 1,
                Width = context.ClientSize.Width,
                Height = context.ClientSize.Height,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.DepthStencil,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None
            });

            // Create the depth buffer view.
            _depthView = new DepthStencilView(context.Direct3DDevice, _depthBuffer);

            var createProps = new CreationProperties();
            _deviceContext = new SharpDX.Direct2D1.DeviceContext(_backBufferSurface, createProps);
        }

        public Surface BackBufferSurface => _backBufferSurface;

        public Texture2D BackBuffer => _backBuffer;

        public RenderTargetView RenderTargetView => _renderView;

        public Texture2D DepthBuffer => _depthBuffer;

        public DepthStencilView DepthView => _depthView;

        public SharpDX.Direct2D1.DeviceContext DeviceContext => _deviceContext;

        public void PushTransform() {
            var transform = _deviceContext.Transform;
            _transformHistory.Push(transform);
        }

        public Matrix3x2 PopTransform() {
            var currentTransform = _deviceContext.Transform;
            var transform = _transformHistory.Pop();
            _deviceContext.Transform = transform;
            return currentTransform;
        }

        protected override void Dispose(bool disposing) {
            if (!disposing) {
                return;
            }
            _deviceContext.Dispose();
            _depthView.Dispose();
            _depthBuffer.Dispose();
            _backBuffer.Dispose();
            _renderView.Dispose();
        }

        private readonly Stack<Matrix3x2> _transformHistory = new Stack<Matrix3x2>();

        private readonly Texture2D _backBuffer;
        private readonly Surface _backBufferSurface;
        private readonly RenderTargetView _renderView;
        private readonly Texture2D _depthBuffer;
        private readonly DepthStencilView _depthView;
        private SharpDX.Direct2D1.DeviceContext _deviceContext;

    }
}
