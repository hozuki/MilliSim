using System;
using System.Drawing;
using System.Windows.Forms;
using OpenMLTD.MilliSim.Foundation;
using SharpDX.Direct3D;
using SharpDX.DXGI;

namespace OpenMLTD.MilliSim.Graphics {
    public sealed class ControlStageRenderer : StageRenderer {

        public ControlStageRenderer(VisualGame game, Control control)
            : base(game) {
            Control = control;
        }

        ~ControlStageRenderer() {
            Control.ClientSizeChanged -= ControlOnClientSizeChanged;
        }

        public Control Control { get; }

        public override Size ClientSize => Control.ClientSize;

        protected override void Dispose(bool disposing) {
            if (disposing) {
                Control.ClientSizeChanged -= ControlOnClientSizeChanged;
            }
            base.Dispose(disposing);
        }

        protected override void CreateSwapChainAndDevice(out SwapChainDescription swapChainDescription, out SwapChain swapChain, out SharpDX.Direct3D11.Device device) {
            var control = Control;
            var clientSize = control.ClientSize;
            var description = new SwapChainDescription {
                BufferCount = 1,
                ModeDescription = new ModeDescription(clientSize.Width, clientSize.Height, DefaultRefreshRate, Format.B8G8R8A8_UNorm),
                IsWindowed = true,
                OutputHandle = control.Handle,
                SampleDescription = new SampleDescription(1, 0),
                SwapEffect = SwapEffect.Discard,
                Usage = Usage.RenderTargetOutput
            };

            SharpDX.Direct3D11.Device.CreateWithSwapChain(DriverType.Hardware, D3DDeviceCreationFlags, description, out device, out swapChain);
            swapChainDescription = description;
        }

        protected override void OnAfterInitialization() {
            base.OnAfterInitialization();
            Control.ClientSizeChanged += ControlOnClientSizeChanged;
        }

        private void ControlOnClientSizeChanged(object sender, EventArgs eventArgs) {
            using (_sizeLock.NewReadLock()) {
                _isSizeChanged = true;
            }
        }

    }
}
