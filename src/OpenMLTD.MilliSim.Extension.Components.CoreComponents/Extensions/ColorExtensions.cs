using Microsoft.Xna.Framework;

namespace OpenMLTD.MilliSim.Extension.Components.CoreComponents.Extensions {
    internal static class ColorExtensions {

        internal static Color ToXna(this System.Drawing.Color color) {
            return new Color(color.R, color.G, color.B, color.A);
        }

    }
}
