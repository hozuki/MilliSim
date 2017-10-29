using System;

namespace OpenMLTD.MilliSim.Foundation {
    public interface IComponent : IDisposable, IUpdateable {

        void OnInitialize();

        void OnDispose();

        string Name { get; }

        GameBase Game { get; }

        IComponentContainer Parent { get; set; }

    }
}
