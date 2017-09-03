using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Graphics;
using OpenMLTD.MilliSim.Graphics.Extensions;
using OpenMLTD.MilliSim.Graphics.Rendering;
using OpenMLTD.MilliSim.Graphics.Rendering.Direct3D;
using OpenMLTD.MilliSim.Graphics.Rendering.Direct3D.Effects;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using Color = System.Drawing.Color;

namespace OpenMLTD.MilliSim.Theater.Elements {
    public class RibbonsLayer : VisualElement {

        public RibbonsLayer(GameBase game)
            : base(game) {
        }

        protected override void OnUpdate(GameTime gameTime) {
            base.OnUpdate(gameTime);
            _camera.UpdateViewMatrix();
        }

        protected override void OnDraw(GameTime gameTime, RenderContext context) {
            base.OnDraw(gameTime, context);

            var viewProjection = _camera.ViewProjectionMatrix;

            context.Begin3D(_posTexLayout, PrimitiveTopology.TriangleList);
            context.Draw(_modelInstance, _textureEffect, viewProjection);
            context.End3D();
        }

        protected override void OnGotContext(RenderContext context) {
            base.OnGotContext(context);

            _camera = new OrthoCamera(context.ClientSize.Width, context.ClientSize.Height, -1000f, 1000f);
            _camera.Position = new Vector3(50, 50, 50);
            _camera.LookAtZUp(Vector3.Zero);

            var settings = Program.Settings;

            _ribbonTexture = Direct3DHelper.LoadTexture2D(context, settings.Images.Ribbon.FileName);
            _ribbonTextureSrv = _ribbonTexture.CreateResourceView();

            var textureEffect = context.CreateD3DEffectFromFile<D3DSimpleTextureEffect>("res/fx/simple_texture.fx");
            _textureEffect = textureEffect;
            textureEffect.WorldTransform = Matrix.Identity;
            textureEffect.Texture = _ribbonTextureSrv;
            textureEffect.TextureTransform = Matrix.Identity;

            var colorEffect = context.CreateD3DEffectFromFile<D3DVertexColorEffect>("res/fx/vertex_color.fx");
            _colorEffect = colorEffect;

            var pass = textureEffect.SimpleTextureTechnique.GetPassByIndex(0);
            _posTexLayout = context.CreateInputLayout(pass, InputLayoutDescriptions.PosNormTex);

            _model = BasicModel.CreateCylinder(context.Direct3DDevice, 60, 60, 60, 100, 200);
            _model.DiffuseMap = _ribbonTextureSrv;
            _model.Material = new D3DMaterial {
                Diffuse = Color.White.ToC4()
            };
            _modelInstance = new BasicModelInstance(_model);
        }

        protected override void OnLostContext(RenderContext context) {
            base.OnLostContext(context);

            _model.Dispose();
            _posTexLayout.Dispose();
            _textureEffect.Dispose();
            _colorEffect.Dispose();
            _ribbonTextureSrv.Dispose();
            _ribbonTexture.Dispose();
        }

        private OrthoCamera _camera;
        private BasicModel _model;
        private BasicModelInstance _modelInstance;
        private Texture2D _ribbonTexture;
        private ShaderResourceView _ribbonTextureSrv;
        private InputLayout _posTexLayout;
        private D3DSimpleTextureEffect _textureEffect;
        private D3DVertexColorEffect _colorEffect;

    }
}
