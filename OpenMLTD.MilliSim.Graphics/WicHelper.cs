using SharpDX.WIC;

namespace OpenMLTD.MilliSim.Graphics {
    public static class WicHelper {

        public static BitmapSource LoadBitmapSourceFromFile(string fileName) {
            using (var factory = new ImagingFactory()) {
                using (var decoder = new BitmapDecoder(factory, fileName, DecodeOptions.CacheOnDemand)) {
                    var converter = new FormatConverter(factory);
                    converter.Initialize(decoder.GetFrame(0), PixelFormat.Format32bppPBGRA, BitmapDitherType.None, null, 0, BitmapPaletteType.Custom);
                    return converter;
                }
            }
        }

    }
}
