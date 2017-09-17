using System;
using System.Drawing;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Core.Entities.Runtime;
using OpenMLTD.MilliSim.Core.Entities.Runtime.Extensions;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Graphics;
using OpenMLTD.MilliSim.Graphics.Extensions;
using OpenMLTD.MilliSim.Graphics.Rendering;
using OpenMLTD.MilliSim.Graphics.Rendering.Direct3D;
using OpenMLTD.MilliSim.Graphics.Rendering.Direct3D.Effects;
using OpenMLTD.MilliSim.Theater.Animation;
using OpenMLTD.MilliSim.Theater.Extensions;
using OpenMLTD.MilliSim.Theater.Internal;
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

            if (_score == null) {
                return;
            }

            var notes = _score.Notes;
            var theaterDays = Game.AsTheaterDays();

            var syncTimer = theaterDays.FindSingleElement<SyncTimer>();
            if (syncTimer == null) {
                return;
            }

            var tapPoints = theaterDays.FindSingleElement<TapPoints>();
            if (tapPoints == null) {
                throw new InvalidOperationException();
            }

            var notesLayer = theaterDays.FindSingleElement<NotesLayer>();
            if (notesLayer == null) {
                throw new InvalidOperationException();
            }

            var gamingArea = theaterDays.FindSingleElement<GamingArea>();
            if (gamingArea == null) {
                throw new InvalidOperationException();
            }

            var settings = Program.Settings;
            var now = syncTimer.CurrentTime.TotalSeconds;
            var tapPointsLayout = settings.UI.TapPoints.Layout;
            var notesLayerLayout = settings.UI.NotesLayer.Layout;
            var clientSize = context.ClientSize;

            var traceCalculator = notesLayer.TraceCalculator;

            var ribbonWidth = gamingArea.ScaleResults.Ribbon.Width;

            var animationMetrics = new NoteAnimationMetrics {
                GlobalSpeedScale = notesLayer.GlobalSpeedScale,
                Width = clientSize.Width,
                Height = clientSize.Height,
                Top = notesLayerLayout.Y * clientSize.Height,
                Bottom = tapPointsLayout.Y * clientSize.Height,
                NoteStartXRatios = tapPoints.StartXRatios,
                NoteEndXRatios = tapPoints.EndXRatios,
                TrackCount = tapPoints.EndXRatios.Length
            };

            var commonNoteMetrics = new NoteMetrics {
                StartRadius = gamingArea.ScaleResults.Note.Start,
                EndRadius = gamingArea.ScaleResults.Note.End
            };

            context.Begin3D(_posTexLayout, PrimitiveTopology.TriangleList);

            foreach (var note in notes) {
                OnStageStatus thisStatus, nextStatus;
                RuntimeNote nextNote;
                RibbonParameters ribbonParams;

                if (note.HasNextHold()) {
                    thisStatus = NoteAnimationHelper.GetOnStageStatusOf(note, now, animationMetrics);

                    if (thisStatus == OnStageStatus.Incoming) {
                        continue;
                    }

                    nextNote = note.NextHold;
                    nextStatus = NoteAnimationHelper.GetOnStageStatusOf(nextNote, now, animationMetrics);

                    if ((int)thisStatus * (int)nextStatus > 0) {
                        continue;
                    }

                    ribbonParams = traceCalculator.GetHoldRibbonParameters(note, nextNote, now, commonNoteMetrics, animationMetrics);
                    using (var mesh = new RibbonMesh(context.Direct3DDevice, SliceCount, ribbonWidth, ribbonParams)) {
                        context.DrawRibbon(mesh, _textureEffect, _ribbonMaterial, _camera.ViewProjectionMatrix, _ribbonTextureSrv);
                    }
                }

                if (note.HasNextSlide()) {
                    thisStatus = NoteAnimationHelper.GetOnStageStatusOf(note, now, animationMetrics);

                    if (thisStatus == OnStageStatus.Incoming) {
                        continue;
                    }

                    nextNote = note.NextSlide;
                    nextStatus = NoteAnimationHelper.GetOnStageStatusOf(nextNote, now, animationMetrics);

                    if ((int)thisStatus * (int)nextStatus > 0) {
                        continue;
                    }

                    ribbonParams = traceCalculator.GetSlideRibbonParameters(note, nextNote, now, commonNoteMetrics, animationMetrics);
                    using (var mesh = new RibbonMesh(context.Direct3DDevice, SliceCount, ribbonWidth, ribbonParams)) {
                        context.DrawRibbon(mesh, _textureEffect, _ribbonMaterial, _camera.ViewProjectionMatrix, _ribbonTextureSrv);
                    }
                }
            }

            context.End3D();
        }

        protected override void OnGotContext(RenderContext context) {
            base.OnGotContext(context);

            var settings = Program.Settings;
            var gamingArea = Game.AsTheaterDays().FindSingleElement<GamingArea>();

            if (gamingArea == null) {
                throw new InvalidOperationException();
            }

            _camera = new OrthoCamera(context.ClientSize.Width, context.ClientSize.Height, -1000, 1000);
            var centerPoint = new PointF(context.ClientSize.Width / 2f, context.ClientSize.Height / 2f);
            _camera.Position = new Vector3(centerPoint.X, centerPoint.Y, 100);
            _camera.LookAt(new Vector3(centerPoint.X, centerPoint.Y, 0), -Vector3.UnitY);

            _ribbonTexture = Direct3DHelper.LoadTexture2D(context, settings.Images.Ribbon.FileName);
            _ribbonTextureSrv = _ribbonTexture.CreateResourceView();

            var textureEffect = context.CreateD3DEffectFromFile<D3DSimpleTextureEffect>("res/fx/simple_texture.fx");
            _textureEffect = textureEffect;
            textureEffect.WorldTransform = Matrix.Identity;
            textureEffect.Texture = _ribbonTextureSrv;
            textureEffect.TextureTransform = Matrix.Identity;

            var pass = textureEffect.SimpleTextureTechnique.GetPassByIndex(0);
            _posTexLayout = context.CreateInputLayout(pass, InputLayoutDescriptions.PosNormTexTan);
        }

        protected override void OnLostContext(RenderContext context) {
            base.OnLostContext(context);

            _posTexLayout?.Dispose();
            _textureEffect?.Dispose();
            _ribbonTextureSrv?.Dispose();
            _ribbonTexture?.Dispose();
        }

        protected override void OnInitialize() {
            base.OnInitialize();

            var scoreLoader = Game.AsTheaterDays().FindSingleElement<ScoreLoader>();
            _score = scoreLoader?.RuntimeScore;

            _ribbonMaterial = new D3DMaterial {
                Diffuse = Color.White.ToC4()
            };
        }

        private static readonly int SliceCount = 48;

        private OrthoCamera _camera;

        private Texture2D _ribbonTexture;
        private ShaderResourceView _ribbonTextureSrv;
        private D3DSimpleTextureEffect _textureEffect;
        private InputLayout _posTexLayout;
        private D3DMaterial _ribbonMaterial;

        private RuntimeScore _score;

    }
}
