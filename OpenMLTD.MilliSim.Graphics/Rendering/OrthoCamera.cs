using System;
using SharpDX;

namespace OpenMLTD.MilliSim.Graphics.Rendering {
    public sealed class OrthoCamera : CameraBase {

        public OrthoCamera(float width, float height, float near, float far) {
            Target = Vector3.Zero;
            Up = Vector3.UnitZ;
            Right = -Vector3.UnitX;
            Look = Vector3.UnitY;
            SetLens(width, height, near, far);
        }

        public Vector3 Target { get; set; }

        public override void LookAt(Vector3 eye, Vector3 target, Vector3 up) {
            Position = eye;
            Target = target;
            Up = up;
            Look = target - eye;
            UpdateViewMatrix();
        }

        public override void Strafe(float d) {
            throw new NotSupportedException();
        }

        public override void Walk(float d) {
            throw new NotSupportedException();
        }

        public override void Pitch(float angle) {
            throw new NotSupportedException();
        }

        public override void Yaw(float angle) {
            throw new NotSupportedException();
        }

        public override void Zoom(float dr) {
            throw new NotSupportedException();
        }

        public override void UpdateViewMatrix() {
            ViewMatrix = Matrix.LookAtLH(Position, Target, Up);
            _frustum = Frustum.FromViewProjection(ViewProjectionMatrix);
        }

        private void SetLens(float width, float height, float near, float far) {
            ProjectionMatrix = Matrix.OrthoLH(width, height, near, far);
            UpdateViewMatrix();
        }

    }
}
