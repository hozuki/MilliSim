using System;

namespace OpenMLTD.TheaterDays {
    internal static class Program {

        /// <summary>
        /// For testing purpose only.
        /// </summary>
        /// <param name="args"></param>
        [STAThread]
        private static int Main(string[] args) {
            Console.WriteLine("This executable is not supposed to be run directly.");
            return -1;
            //return Theater.Run(args, GraphicsBackend.Direct3D11);
        }

    }
}
