using SharpDX;

namespace OpenMLTD.MilliSim.Graphics.Rendering {
    public sealed class LookAtCamera : CameraBase {

        public LookAtCamera(float fovY, float aspect, float near, float far) {
            _alpha = _beta = 0.5f;
            _radius = 10.0f;
            Right = -Vector3.UnitX;
            Up = Vector3.UnitZ;
            Look = Vector3.UnitY;
            Target = Vector3.Zero;
            SetLens(fovY, aspect, near, far);
        }

        public Vector3 Target { get; set; }

        public override void LookAt(Vector3 eye, Vector3 target, Vector3 up) {
            Target = target;
            Position = eye;
            Position = eye;
            Look = Vector3.Normalize(target - eye);
            Right = Vector3.Normalize(Vector3.Cross(up, Look));
            Up = Vector3.Cross(Look, Right);
            _radius = (target - eye).Length();
        }

        public override void Strafe(float d) {
            var dt = Vector3.Normalize(new Vector3(Right.X, 0, Right.Z)) * d;
            Target += dt;
        }

        public override void Walk(float d) {
            Target += Vector3.Normalize(new Vector3(Look.X, 0, Look.Z)) * d;
        }

        public override void Pitch(float angle) {
            _beta += angle;
            _beta = MathF.Clamp(_beta, 0.05f, MathF.PI / 2.0f - 0.01f);
        }

        public override void Yaw(float angle) {
            _alpha = (_alpha + angle) % (MathF.PI * 2.0f);
        }
        public override void Zoom(float dr) {
            _radius += dr;
            _radius = MathF.Clamp(_radius, 2.0f, 150.0f);
        }

        public HeightFunc HeightFunc { get; set; }

        public override void UpdateViewMatrix() {
            var sideRadius = _radius * MathF.Cos(_beta);
            var height = _radius * MathF.Sin(_beta);

            Position = new Vector3(
                Target.X + sideRadius * MathF.Cos(_alpha),
                Target.Y + height,
                Target.Z + sideRadius * MathF.Sin(_alpha)
            );
            if (HeightFunc != null && Position.Y <= HeightFunc(Position.X, Position.Z) + 2.0f) {
                Position = new Vector3(Position.X, HeightFunc(Position.X, Position.Z) + 2.0f, Position.Z);
            }

            ViewMatrix = Matrix.LookAtLH(Position, Target, Vector3.UnitY);

            Right = new Vector3(ViewMatrix.M11, ViewMatrix.M21, ViewMatrix.M31);
            Right.Normalize();

            Look = new Vector3(ViewMatrix.M13, ViewMatrix.M23, ViewMatrix.M33);
            Look.Normalize();
            _frustum = Frustum.FromViewProjection(ViewProjectionMatrix);
        }

        private void SetLens(float fovY, float aspect, float near, float far) {
            FovY = fovY;
            Aspect = aspect;
            NearZ = near;
            FarZ = far;
            NearWindowHeight = 2.0f * NearZ * MathF.Tan(0.5f * FovY);
            FarWindowHeight = 2.0f * FarZ * MathF.Tan(0.5f * FovY);
            ProjectionMatrix = Matrix.PerspectiveFovLH(FovY, Aspect, NearZ, FarZ);
        }

        private float _radius;
        private float _alpha;
        private float _beta;

    }
}
