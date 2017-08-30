using OpenMLTD.MilliSim.Core;

namespace OpenMLTD.MilliSim.Rendering {
    public interface IUpdateable {

        void Update(GameTime gameTime);

        bool Enabled { get; set; }

    }
}
