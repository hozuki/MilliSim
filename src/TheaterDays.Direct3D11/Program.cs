using System;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Graphics;

namespace OpenMLTD.TheaterDays {
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    internal static class Program {

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static int Main([NotNull] string[] args) {
            return Theater.Run(args, GraphicsBackend.Direct3D11);
        }

    }
#endif
}
