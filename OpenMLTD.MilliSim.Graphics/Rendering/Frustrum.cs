using SharpDX;

namespace OpenMLTD.MilliSim.Graphics.Rendering {
    public class Frustum {

        public Frustum(Matrix viewProjection) {
            _planes = new[] {
                //left
                new Plane(viewProjection.M14 + viewProjection.M11, viewProjection.M24 + viewProjection.M21, viewProjection.M34 + viewProjection.M31, viewProjection.M44 + viewProjection.M41),
                // right
                new Plane(viewProjection.M14 - viewProjection.M11, viewProjection.M24 - viewProjection.M21, viewProjection.M34 - viewProjection.M31, viewProjection.M44 - viewProjection.M41),
                // bottom
                new Plane(viewProjection.M14 + viewProjection.M12, viewProjection.M24 + viewProjection.M22, viewProjection.M34 + viewProjection.M32, viewProjection.M44 + viewProjection.M42),
                // top
                new Plane(viewProjection.M14 - viewProjection.M12, viewProjection.M24 - viewProjection.M22, viewProjection.M34 - viewProjection.M32, viewProjection.M44 - viewProjection.M42),
                //near
                new Plane(viewProjection.M13, viewProjection.M23, viewProjection.M33, viewProjection.M43),
                //far
                new Plane(viewProjection.M14 - viewProjection.M13, viewProjection.M24 - viewProjection.M23, viewProjection.M34 - viewProjection.M33, viewProjection.M44 - viewProjection.M43)
            };
            foreach (var plane in Planes) {
                plane.Normalize();
            }
        }

        public Plane[] Planes => _planes;

        public static Frustum FromViewProjection(Matrix viewProjection) {
            return new Frustum(viewProjection);
        }

        public IntersectionState Intersects(BoundingBox box) {
            var totalIn = 0;

            for (var i = 0; i < _planes.Length; ++i) {
                var plane = _planes[i];
                var intersection = plane.Intersects(ref box);
                switch (intersection) {
                    case PlaneIntersectionType.Back:
                        return IntersectionState.NoIntersection;
                    case PlaneIntersectionType.Front:
                        totalIn++;
                        break;
                    default:
                        break;
                }
            }
            return totalIn >= 6 ? IntersectionState.ClientInsideHost : IntersectionState.PartialIntersection;
        }

        private readonly Plane[] _planes;

    }
}
