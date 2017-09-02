using SharpDX;

namespace OpenMLTD.MilliSim.Graphics.Rendering {
    public abstract class CameraBase {

        public Vector3 Position { get; set; }

        public Vector3 Right { get; set; }

        public Vector3 Up { get; set; }

        public Vector3 Look { get; protected set; }

        public float NearZ { get; protected set; }

        public float FarZ { get; protected set; }

        public virtual float Aspect {
            get => _aspect;
            set => _aspect = value;
        }

        public float FovY {
            get => _fovY;
            protected set => _fovY = value;
        }

        public float FovX {
            get {
                var halfWidth = 0.5f * NearWindowWidth;
                return 2.0f * MathF.Atan(halfWidth / NearZ);
            }
        }

        public float NearWindowWidth => Aspect * NearWindowHeight;

        public float NearWindowHeight { get; protected set; }

        public float FarWindowWidth => Aspect * FarWindowHeight;

        public float FarWindowHeight { get; protected set; }

        public Matrix ViewMatrix {
            get => _viewMatrix;
            protected set {
                _viewMatrix = value;
                _viewProjectionMatrix = _viewMatrix * _projectionMatrix;
            }
        }

        public Matrix ProjectionMatrix {
            get => _projectionMatrix;
            protected set {
                _projectionMatrix = value;
                _viewProjectionMatrix = _viewMatrix * _projectionMatrix;
            }
        }

        public Matrix ViewProjectionMatrix => _viewProjectionMatrix;

        public Plane[] FrustumPlanes => _frustum.Planes;

        public abstract void LookAt(Vector3 eye, Vector3 target, Vector3 up);
        public abstract void Strafe(float rightDistance);
        public abstract void Walk(float frontDistance);
        public abstract void Pitch(float angle);
        public abstract void Yaw(float angle);
        public abstract void Zoom(float dr);

        public void LookAt(Vector3 target, Vector3 up) {
            LookAt(Position, target, up);
        }

        public void LookAt(Vector3 target) {
            LookAt(Position, target, Up);
        }

        public void LookAtZUp(Vector3 target) {
            LookAt(Position, target, Vector3.UnitZ);
        }

        public bool IsBoundingBoxVisible(BoundingBox box) => _frustum.Intersects(box) != IntersectionState.NoIntersection;

        public abstract void UpdateViewMatrix();

        protected CameraBase() {
            Position = Vector3.Zero;
            _viewMatrix = Matrix.Identity;
            _projectionMatrix = Matrix.Identity;
            _viewProjectionMatrix = Matrix.Identity;
        }

        protected Frustum _frustum;
        protected float _aspect;
        protected float _fovY;

        private Matrix _viewMatrix;
        private Matrix _projectionMatrix;
        private Matrix _viewProjectionMatrix;

    }
}
