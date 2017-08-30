namespace OpenMLTD.MilliSim.Rendering {
    public interface IElement : IUpdateable {

        void Initialize();

        string Name { get; }

    }
}
