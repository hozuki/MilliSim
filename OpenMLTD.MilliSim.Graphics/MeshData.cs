using System.Collections.Generic;

namespace OpenMLTD.MilliSim.Graphics {
    public class MeshData {

        public MeshData(IReadOnlyList<MeshVertex> vertices, IReadOnlyList<int> indices) {
            Vertices = vertices;
            Indices = indices;
        }

        public IReadOnlyList<MeshVertex> Vertices { get; }

        public IReadOnlyList<int> Indices { get; }

    }
}
