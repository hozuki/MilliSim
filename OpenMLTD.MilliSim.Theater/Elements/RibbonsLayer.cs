using System;
using System.Collections.Generic;
using System.Drawing;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Core.Entities;
using OpenMLTD.MilliSim.Core.Entities.Extensions;
using OpenMLTD.MilliSim.Core.Entities.Runtime;
using OpenMLTD.MilliSim.Core.Entities.Source;
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

            var topYRatio = _ribbonTopYRatio;
            var bottomYRatio = _ribbonBottomYRatio;

            // Enable depth buffer to allow drawing in layers.
            // However the depth comparison always passes, and Z always decreases (see below), so the results looks like
            // "later-drawn-on-top" inside this visual element.
            // We are using Begin3DFast() here, so the states are specified in effect file (simple_texture.fx). Please
            // open that file to see which settings we are selecting.
            context.Begin3DFast(_posTexLayout, PrimitiveTopology.TriangleList);

            // Z value should be decreasing to achieve this effect:
            // when two ribbons cross, the one with the start note on the right is above the other.
            var z = 0f;

            var smallNoteMetrics = new NoteMetrics {
                StartRadius = gamingArea.ScaleResults.VisualNoteSmall.Start,
                EndRadius = gamingArea.ScaleResults.VisualNoteSmall.End
            };
            var largeNoteMetrics = new NoteMetrics {
                StartRadius = gamingArea.ScaleResults.VisualNoteLarge.Start,
                EndRadius = gamingArea.ScaleResults.VisualNoteLarge.End
            };

            foreach (var note in notes) {
                OnStageStatus thisStatus, nextStatus;
                RuntimeNote nextNote;
                NoteMetrics visualNoteMetrics;

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

                    var firstHoldInGroup = note;
                    while (firstHoldInGroup.HasPrevHold()) {
                        firstHoldInGroup = firstHoldInGroup.PrevHold;
                    }
                    visualNoteMetrics = firstHoldInGroup.IsFlick() || firstHoldInGroup.Size == NoteSize.Large ? largeNoteMetrics : smallNoteMetrics;

                    var ribbonParams = traceCalculator.GetHoldRibbonParameters(note, nextNote, now, visualNoteMetrics, animationMetrics);
                    using (var mesh = new RibbonMesh(context.Direct3DDevice, SliceCount, topYRatio, bottomYRatio, z, new[] { ribbonParams }, traceCalculator, now, new[] { (note, note.NextHold) }, visualNoteMetrics, animationMetrics)) {
                        context.DrawRibbon(mesh, _textureEffect, _ribbonMaterial, _camera.ViewProjectionMatrix, _ribbonTextureSrv);
                    }

                    z -= 0.1f;
                }

                if (note.HasNextSlide()) {
                    thisStatus = NoteAnimationHelper.GetOnStageStatusOf(note, now, animationMetrics);

                    if (thisStatus == OnStageStatus.Incoming) {
                        continue;
                    }

                    nextNote = note.NextSlide;
                    nextStatus = NoteAnimationHelper.GetOnStageStatusOf(nextNote, now, animationMetrics);

                    if (thisStatus == OnStageStatus.Visible && note.HasPrevSlide()) {
                        // We have already drawn this slide group.
                        continue;
                    }

                    if ((int)thisStatus * (int)nextStatus > 0) {
                        continue;
                    }

                    var firstSlideInGroup = note;
                    while (firstSlideInGroup.HasPrevSlide()) {
                        firstSlideInGroup = firstSlideInGroup.PrevSlide;
                    }
                    visualNoteMetrics = firstSlideInGroup.IsFlick() || firstSlideInGroup.Size == NoteSize.Large ? largeNoteMetrics : smallNoteMetrics;

                    if (nextStatus == OnStageStatus.Visible && nextNote.HasNextSlide()) {
                        var nextSlides = new List<RuntimeNote>();
                        nextSlides.Add(nextNote);

                        do {
                            nextNote = nextNote.NextSlide;
                            nextStatus = NoteAnimationHelper.GetOnStageStatusOf(nextNote, now, animationMetrics);
                            nextSlides.Add(nextNote);
                        } while (nextNote.HasNextSlide() && nextStatus > OnStageStatus.Incoming);

                        var slideNotePairCount = nextSlides.Count;
                        var ribbonParamArray = new RibbonParameters[slideNotePairCount];
                        var slideNotePairs = new(RuntimeNote, RuntimeNote)[slideNotePairCount];
                        for (var i = 0; i < slideNotePairCount; ++i) {
                            if (i == 0) {
                                slideNotePairs[i] = (note, nextSlides[0]);
                                ribbonParamArray[i] = traceCalculator.GetSlideRibbonParameters(note, nextSlides[0], now, visualNoteMetrics, animationMetrics);
                            } else {
                                slideNotePairs[i] = (nextSlides[i - 1], nextSlides[i]);
                                ribbonParamArray[i] = traceCalculator.GetSlideRibbonParameters(nextSlides[i - 1], nextSlides[i], now, visualNoteMetrics, animationMetrics);
                            }
                        }

                        using (var mesh = new RibbonMesh(context.Direct3DDevice, SliceCount, topYRatio, bottomYRatio, z, ribbonParamArray, traceCalculator, now, slideNotePairs, visualNoteMetrics, animationMetrics)) {
                            context.DrawRibbon(mesh, _textureEffect, _ribbonMaterial, _camera.ViewProjectionMatrix, _ribbonTextureSrv);
                        }

                        z -= 0.1f;
                    } else {
                        var ribbonParams = traceCalculator.GetSlideRibbonParameters(note, nextNote, now, visualNoteMetrics, animationMetrics);

                        using (var mesh = new RibbonMesh(context.Direct3DDevice, SliceCount, topYRatio, bottomYRatio, z, new[] { ribbonParams }, traceCalculator, now, new[] { (note, note.NextSlide) }, visualNoteMetrics, animationMetrics)) {
                            context.DrawRibbon(mesh, _textureEffect, _ribbonMaterial, _camera.ViewProjectionMatrix, _ribbonTextureSrv);
                        }

                        z -= 0.1f;
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

            _camera = new OrthoCamera(context.ClientSize.Width, context.ClientSize.Height, -1, 1000);
            var centerPoint = new PointF(context.ClientSize.Width / 2f, context.ClientSize.Height / 2f);
            _camera.Position = new Vector3(centerPoint.X, centerPoint.Y, 1);
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

            var textureDesc = _ribbonTexture.Description;
            var textureBlankEdges = settings.Images.Ribbon.BlankEdge;
            _ribbonTopYRatio = textureBlankEdges.Top / textureDesc.Height;
            _ribbonBottomYRatio = (textureDesc.Height - textureBlankEdges.Bottom) / textureDesc.Height;
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

        private float _ribbonTopYRatio;
        private float _ribbonBottomYRatio;

        private RuntimeScore _score;

    }
}
