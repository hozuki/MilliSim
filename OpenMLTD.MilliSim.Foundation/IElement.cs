namespace OpenMLTD.MilliSim.Foundation {
    public interface IElement : IUpdateable {

        void Initialize();

        string Name { get; }

    }
}
