namespace OpenMLTD.MilliSim.Foundation.Extensions {
    public static class UpdatableExtensions {

        public static void Enable(this IUpdateable @this) {
            @this.Enabled = true;
        }

        public static void Disable(this IUpdateable @this) {
            @this.Enabled = false;
        }

    }
}
