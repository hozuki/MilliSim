using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Graphics;
using OpenMLTD.MilliSim.Graphics.Extensions;
using OpenMLTD.MilliSim.Graphics.Rendering;
using OpenMLTD.MilliSim.Graphics.Rendering.Direct3D;
using OpenMLTD.MilliSim.Graphics.Rendering.Direct3D.Effects;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Buffer = SharpDX.Direct3D11.Buffer;
using Color = System.Drawing.Color;

namespace OpenMLTD.MilliSim.Extension.Components.CoreComponents {
    public sealed class BackgroundImage : BackgroundBase {

        public BackgroundImage([NotNull] IVisualContainer parent)
            : base(parent) {
        }

        public void Load([NotNull] string path) {
            _filePath = path;
        }

        public void Unload() {
            _filePath = null;
        }

        protected override void OnUpdate(GameTime gameTime) {
            base.OnUpdate(gameTime);

            if (_camera != null) {
                _camera.UpdateViewMatrix();
            }
        }

        protected override void OnDraw(GameTime gameTime, RenderContext context) {
            base.OnDraw(gameTime, context);

            if (_filePath != null) {
                if (_ribbonTexture == null) {
                    OnGotContext(context);
                }
            } else {
                if (_ribbonTexture != null) {
                    OnLostContext(context);
                }
            }

            if (_ribbonTexture != null) {
                var viewProjection = _camera.ViewProjectionMatrix;
                var location = Location;
                var world = Matrix.Translation(location.X, location.Y, 0);
                var worldInvTrans = MathF.InverseTranspose(world);
                var worldViewProjection = world * viewProjection;

                _textureEffect.WorldTransform = world;
                _textureEffect.WorldTransformInversedTransposed = worldInvTrans;
                _textureEffect.WorldViewProjectionTransform = worldViewProjection;

                var tech = _textureEffect.SimpleTextureTechnique;
                var pass = tech.GetPassByIndex(0);

                context.Begin3D(_posTexLayout, PrimitiveTopology.TriangleList);

                var immediateContext = context.Direct3DDevice.ImmediateContext;
                pass.Apply(immediateContext);

                immediateContext.InputAssembler.SetIndexBuffer(_indexBuffer, Format.R16_UInt, 0);
                immediateContext.InputAssembler.SetVertexBuffers(0, _vertexBufferBinding);

                immediateContext.DrawIndexed(6, 0, 0);

                context.End3D();
            }
        }

        protected override void OnGotContext(RenderContext context) {
            base.OnGotContext(context);

            if (_filePath == null || !File.Exists(_filePath)) {
                return;
            }

            var clientSize = context.ClientSize;

            _camera = new OrthoCamera(clientSize.Width, clientSize.Height, 0.1f, ViewFrustrumDepth);
            var centerPoint = new PointF(context.ClientSize.Width / 2f, context.ClientSize.Height / 2f);
            _camera.Position = new Vector3(centerPoint.X, centerPoint.Y, CameraZ);
            _camera.LookAt(new Vector3(centerPoint.X, centerPoint.Y, 0), -Vector3.UnitY);

            _ribbonTexture = Direct3DHelper.LoadTexture2D(context, _filePath);
            _ribbonTextureSrv = _ribbonTexture.CreateResourceView();

            var textureEffect = context.CreateD3DEffectFromFile<D3DSimpleTextureEffect>("res/fx/simple_texture.fx");
            _textureEffect = textureEffect;
            textureEffect.WorldTransform = Matrix.Identity;
            textureEffect.Texture = _ribbonTextureSrv;
            textureEffect.TextureTransform = Matrix.Identity;
            textureEffect.Material = _ribbonMaterial;

            var pass = textureEffect.SimpleTextureTechnique.GetPassByIndex(0);
            _posTexLayout = context.CreateInputLayout(pass, InputLayoutDescriptions.PosNormTexTan);

            var textureSize = new SizeF(_ribbonTexture.Description.Width, _ribbonTexture.Description.Height);

            var vertices = new[] {
                new MeshVertex(0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0),
                new MeshVertex(textureSize.Width, 0, 0, 0, 0, 1, 1, 0, 0, 1, 0),
                new MeshVertex(0, textureSize.Height, 0, 0, 0, 1, 1, 0, 0, 0, 1),
                new MeshVertex(textureSize.Width, textureSize.Height, 0, 0, 0, 1, 1, 0, 0, 1, 1)
            };
            var indices = new ushort[] {
                0, 1, 2, 3, 2, 1
            };

            var device = context.Direct3DDevice;

            var vertexDataSize = vertices.Length * Marshal.SizeOf(typeof(MeshVertex));
            var vertexDataStream = DataStream.Create(vertices, false, false);
            _vertexDataStream = vertexDataStream;
            _vertexBuffer = new Buffer(device, vertexDataStream, vertexDataSize, ResourceUsage.Immutable, BindFlags.VertexBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, Marshal.SizeOf(typeof(MeshVertex)));

            var indexDataSize = indices.Length * Marshal.SizeOf(typeof(ushort));
            var indexDataStream = DataStream.Create(indices, false, false);
            _indexDataStream = indexDataStream;
            _indexBuffer = new Buffer(device, indexDataStream, indexDataSize, ResourceUsage.Immutable, BindFlags.IndexBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, Marshal.SizeOf(typeof(ushort)));

            _vertexBufferBinding = new VertexBufferBinding(_vertexBuffer, Marshal.SizeOf(typeof(MeshVertex)), 0);
        }

        protected override void OnLostContext(RenderContext context) {
            base.OnLostContext(context);

            _indexDataStream?.Dispose();
            _vertexDataStream?.Dispose();
            _indexBuffer?.Dispose();
            _vertexBuffer?.Dispose();
            _posTexLayout?.Dispose();
            _textureEffect?.Dispose();
            _ribbonTextureSrv?.Dispose();
            _ribbonTexture?.Dispose();
        }

        protected override void OnInitialize() {
            base.OnInitialize();

            _ribbonMaterial = new D3DMaterial {
                Diffuse = Color.White.ToC4()
            };
        }

        private string _filePath;

        private static readonly float CameraZ = 1f;
        private static readonly float ViewFrustrumDepth = 100f;

        private OrthoCamera _camera;

        private Texture2D _ribbonTexture;
        private ShaderResourceView _ribbonTextureSrv;
        private D3DSimpleTextureEffect _textureEffect;
        private InputLayout _posTexLayout;
        private D3DMaterial _ribbonMaterial;

        private Buffer _indexBuffer;
        private Buffer _vertexBuffer;
        private DataStream _indexDataStream;
        private DataStream _vertexDataStream;
        private VertexBufferBinding _vertexBufferBinding;

    }
}
