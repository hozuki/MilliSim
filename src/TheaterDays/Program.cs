using OpenMLTD.MilliSim.Graphics;

namespace OpenMLTD.TheaterDays {
    internal static class Program {

        /// <summary>
        /// For testing purpose only.
        /// </summary>
        /// <param name="args"></param>
        private static void Main(string[] args) {
            Theater.Run(args, GraphicsBackend.OpenGL);
        }

    }
}
