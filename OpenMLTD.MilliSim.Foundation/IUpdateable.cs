using OpenMLTD.MilliSim.Core;

namespace OpenMLTD.MilliSim.Foundation {
    public interface IUpdateable {

        void Update(GameTime gameTime);

        bool Enabled { get; set; }

    }
}
