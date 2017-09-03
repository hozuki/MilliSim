using OpenMLTD.MilliSim.Core;
using SharpDX;
using SharpDX.Direct3D11;

namespace OpenMLTD.MilliSim.Graphics.Rendering {
    public sealed class FrequentlyUsedStates : DisposableBase {

        internal FrequentlyUsedStates(RenderContext context) {
            _context = context;
            Initialize();
        }

        public RasterizerState Wireframe => _wireframe;

        public RasterizerState NoCull => _noCull;

        public RasterizerState CullClockwise => _cullClockwise;

        public RasterizerState CullCounterclockwise => _cullCounterclockwise;

        public BlendState AlphaBlend => _alphaBlend;

        public BlendState Transparent => _transparent;

        public BlendState Opaque => _opaque;

        protected override void Dispose(bool disposing) {
            if (!disposing) {
                return;
            }

            Utilities.Dispose(ref _noCull);
            Utilities.Dispose(ref _cullClockwise);
            Utilities.Dispose(ref _cullCounterclockwise);

            Utilities.Dispose(ref _alphaBlend);
            Utilities.Dispose(ref _transparent);
            Utilities.Dispose(ref _opaque);
        }

        private void Initialize() {
            var device = _context.Direct3DDevice;
            InitializeRasterizerStates(device);
            InitializeBlendStates(device);
        }

        private void InitializeRasterizerStates(Device device) {
            var wireframeDesc = new RasterizerStateDescription {
                FillMode = FillMode.Wireframe,
                CullMode = CullMode.Back,
                IsFrontCounterClockwise = false,
                IsDepthClipEnabled = true
            };
            _wireframe = new RasterizerState(device, wireframeDesc);

            var noCullDesc = new RasterizerStateDescription {
                FillMode = FillMode.Solid,
                CullMode = CullMode.None,
                IsFrontCounterClockwise = false,
                IsDepthClipEnabled = true
            };
            _noCull = new RasterizerState(device, noCullDesc);

            var cullClockwiseDesc = new RasterizerStateDescription {
                FillMode = FillMode.Solid,
                CullMode = CullMode.Back,
                IsFrontCounterClockwise = true,
                IsDepthClipEnabled = true
            };
            _cullClockwise = new RasterizerState(device, cullClockwiseDesc);

            var cullCounterclockwiseDesc = new RasterizerStateDescription {
                FillMode = FillMode.Solid,
                CullMode = CullMode.Back,
                IsFrontCounterClockwise = false,
                IsDepthClipEnabled = true
            };
            _cullCounterclockwise = new RasterizerState(device, cullCounterclockwiseDesc);
        }

        private void InitializeBlendStates(Device device) {
            var alphaBlendDesc = new BlendStateDescription {
                AlphaToCoverageEnable = false,
                IndependentBlendEnable = false
            };
            alphaBlendDesc.RenderTarget[0].IsBlendEnabled = true;
            alphaBlendDesc.RenderTarget[0].SourceBlend = BlendOption.SourceAlpha;
            alphaBlendDesc.RenderTarget[0].DestinationBlend = BlendOption.InverseSourceAlpha;
            alphaBlendDesc.RenderTarget[0].BlendOperation = BlendOperation.Add;
            alphaBlendDesc.RenderTarget[0].SourceAlphaBlend = BlendOption.One;
            alphaBlendDesc.RenderTarget[0].DestinationAlphaBlend = BlendOption.Zero;
            alphaBlendDesc.RenderTarget[0].AlphaBlendOperation = BlendOperation.Add;
            alphaBlendDesc.RenderTarget[0].RenderTargetWriteMask = ColorWriteMaskFlags.All;
            _alphaBlend = new BlendState(device, alphaBlendDesc);

            var transparentDesc = new BlendStateDescription {
                AlphaToCoverageEnable = false,
                IndependentBlendEnable = false
            };
            transparentDesc.RenderTarget[0].IsBlendEnabled = false;
            transparentDesc.RenderTarget[0].SourceBlend = BlendOption.One;
            transparentDesc.RenderTarget[0].DestinationBlend = BlendOption.Zero;
            transparentDesc.RenderTarget[0].BlendOperation = BlendOperation.Add;
            transparentDesc.RenderTarget[0].SourceAlphaBlend = BlendOption.One;
            transparentDesc.RenderTarget[0].DestinationAlphaBlend = BlendOption.Zero;
            transparentDesc.RenderTarget[0].AlphaBlendOperation = BlendOperation.Add;
            transparentDesc.RenderTarget[0].RenderTargetWriteMask = 0;
            _transparent = new BlendState(device, transparentDesc);

            var opaqueDesc = new BlendStateDescription {
                AlphaToCoverageEnable = true,
                IndependentBlendEnable = false,
            };
            opaqueDesc.RenderTarget[0].IsBlendEnabled = false;
            opaqueDesc.RenderTarget[0].RenderTargetWriteMask = ColorWriteMaskFlags.All;
            _opaque = new BlendState(device, opaqueDesc);
        }

        private RasterizerState _wireframe;
        private RasterizerState _noCull;
        private RasterizerState _cullClockwise;
        private RasterizerState _cullCounterclockwise;

        private BlendState _alphaBlend;
        private BlendState _transparent;
        private BlendState _opaque;

        private readonly RenderContext _context;

    }
}
