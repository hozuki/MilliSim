using System;
using Microsoft.Xna.Framework;
using OpenMLTD.MilliSim.Configuration;

namespace OpenMLTD.MilliSim.Foundation {
    public interface IBaseGameComponent : IGameComponent, IDisposable, IUpdateable {

        string Name { get; }

        IBaseGameComponentContainer Parent { get; set; }

        ConfigurationStore ConfigurationStore { get; }

        void LoadContents();

        void UnloadContents();

        bool AreContentsLoaded { get; }

    }
}
