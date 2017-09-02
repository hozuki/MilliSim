using SharpDX;

namespace OpenMLTD.MilliSim.Graphics.Rendering {
    public sealed class FpsCamera : CameraBase {

        public FpsCamera(float fovY, float aspect, float near, float far) {
            Right = -Vector3.UnitX;
            Up = Vector3.UnitZ;
            Look = Vector3.UnitY;
            SetLens(fovY, aspect, near, far);
        }

        public override void LookAt(Vector3 eye, Vector3 target, Vector3 up) {
            Position = eye;
            Look = Vector3.Normalize(target - eye);
            Right = Vector3.Normalize(Vector3.Cross(up, Look));
            Up = Vector3.Cross(Look, Right);
        }

        public override void Pitch(float angle) {
            var r = Matrix.RotationAxis(Right, angle);
            Up = Vector3.TransformNormal(Up, r);
            Look = Vector3.TransformNormal(Look, r);
        }

        public override void Strafe(float rightDistance) {
            Position += Right * rightDistance;
        }

        public override void Walk(float frontDistance) {
            Position += Look * frontDistance;
        }

        public override void Yaw(float angle) {
            var r = Matrix.RotationY(angle);
            Right = Vector3.TransformNormal(Right, r);
            Up = Vector3.TransformNormal(Up, r);
            Look = Vector3.TransformNormal(Look, r);
        }

        public override void Zoom(float dr) {
            var newFov = MathF.Clamp(FovY + dr, 0.1f, MathF.PI / 2);
            SetLens(newFov, Aspect, NearZ, FarZ);
        }

        public override float Aspect {
            get { return _aspect; }
            set {
                _aspect = value;
                ProjectionMatrix = Matrix.PerspectiveFovLH(FovY, Aspect, NearZ, FarZ);
            }
        }

        public override void UpdateViewMatrix() {
            var r = Right;
            var l = Look;
            var p = Position;

            l = Vector3.Normalize(l);
            var u = Vector3.Normalize(Vector3.Cross(l, r));
            r = Vector3.Cross(u, l);

            var x = -Vector3.Dot(p, r);
            var y = -Vector3.Dot(p, u);
            var z = -Vector3.Dot(p, l);

            Right = r;
            Up = u;
            Look = l;

            ViewMatrix = new Matrix {
                Row1 = new Vector4(r.X, u.X, l.X, 0),
                Row2 = new Vector4(r.Y, u.Y, l.Y, 0),
                Row3 = new Vector4(r.Z, u.Z, l.Z, 0),
                Row4 = new Vector4(x, y, z, 1)
            };

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

    }
}
