using System;

namespace OpenMLTD.MilliSim.Foundation {
    public interface IElement : IDisposable, IUpdateable {

        void OnInitialize();

        void OnDispose();

        string Name { get; }

    }
}
