using SharpDX;

namespace OpenMLTD.MilliSim.Graphics.Rendering {
    public sealed class BasicModelInstance {

        public BasicModelInstance(BasicModel model) {
            Model = model;
            World = Matrix.Identity;
        }

        public BasicModel Model { get; }

        public Matrix World { get; set; }

    }
}
