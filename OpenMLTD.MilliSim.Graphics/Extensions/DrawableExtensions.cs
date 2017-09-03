namespace OpenMLTD.MilliSim.Graphics.Extensions {
    public static class DrawableExtensions {

        public static void Show(this IDrawable @this) {
            @this.Visible = true;
        }

        public static void Hide(this IDrawable @this) {
            @this.Visible = false;
        }

    }
}
