using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.WIC;
using Device = SharpDX.Direct3D11.Device;

namespace OpenMLTD.MilliSim.Graphics.Rendering {
    public static class Direct3DHelper {

        public static Texture2D LoadTexture2D(RenderContext context, string fileName) {
            return LoadTexture2D(context.Direct3DDevice, fileName, NormalTextureOptions);
        }

        public static Texture2D LoadTexture2D(Device device, string fileName) {
            return LoadTexture2D(device, fileName, NormalTextureOptions);
        }

        public static Texture2D LoadTexture2D(RenderContext context, string fileName, Texture2DLoadOptions options) {
            return LoadTexture2D(context.Direct3DDevice, fileName, options);
        }

        public static Texture2D LoadTexture2D(Device device, string fileName, Texture2DLoadOptions options) {
            using (var bitmapSource = WicHelper.LoadBitmapSourceFromFile(fileName)) {
                return CreateTexture2DFromBitmapSource(device, bitmapSource, options);
            }
        }

        private static Texture2D CreateTexture2DFromBitmapSource(Device device, BitmapSource bitmapSource, Texture2DLoadOptions options) {
            var stride = bitmapSource.Size.Width * 4;
            // Allocate DataStream to receive the WIC image pixels
            using (var buffer = new DataStream(bitmapSource.Size.Height * stride, true, true)) {
                // Copy the content of the WIC to the buffer
                bitmapSource.CopyPixels(stride, buffer);
                var texture2DDescription = new Texture2DDescription {
                    Width = bitmapSource.Size.Width,
                    Height = bitmapSource.Size.Height,
                    ArraySize = 1,
                    BindFlags = options.BindFlags,
                    Usage = options.ResourceUsage,
                    CpuAccessFlags = options.CpuAccessFlags,
                    Format = options.Format,
                    MipLevels = options.MipLevels,
                    OptionFlags = ResourceOptionFlags.None,
                    SampleDescription = new SampleDescription(1, 0)
                };
                bitmapSource.Dispose();
                var dataRectangle = new DataRectangle(buffer.DataPointer, stride);
                return new Texture2D(device, texture2DDescription, dataRectangle);
            }
        }

        private static readonly Texture2DLoadOptions NormalTextureOptions = new Texture2DLoadOptions {
            Format = Format.B8G8R8A8_UNorm,
            BindFlags = BindFlags.ShaderResource,
            CpuAccessFlags = CpuAccessFlags.None,
            ResourceUsage = ResourceUsage.Immutable,
            MipLevels = 1
        };

    }
}
