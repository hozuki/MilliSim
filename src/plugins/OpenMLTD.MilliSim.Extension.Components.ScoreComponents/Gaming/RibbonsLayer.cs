using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpenMLTD.MilliSim.Contributed.Scores;
using OpenMLTD.MilliSim.Contributed.Scores.Animation;
using OpenMLTD.MilliSim.Contributed.Scores.Extensions;
using OpenMLTD.MilliSim.Contributed.Scores.Runtime;
using OpenMLTD.MilliSim.Extension.Components.CoreComponents;
using OpenMLTD.MilliSim.Extension.Components.ScoreComponents.Configuration;
using OpenMLTD.MilliSim.Extension.Components.ScoreComponents.Effects;
using OpenMLTD.MilliSim.Extension.Components.ScoreComponents.Extensions;
using OpenMLTD.MilliSim.Extension.Components.ScoreComponents.Internal;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Foundation.Extensions;
using OpenMLTD.MilliSim.Graphics;

namespace OpenMLTD.MilliSim.Extension.Components.ScoreComponents.Gaming {
    public class RibbonsLayer : Visual {

        public RibbonsLayer([NotNull] BaseGame game, [NotNull] IVisualContainer parent)
            : base(game, parent) {
        }

        protected override void OnDraw(GameTime gameTime) {
            base.OnDraw(gameTime);

            var theaterDays = Game.ToBaseGame();
            var scoreLoader = theaterDays.FindSingleElement<ScoreLoader>();

            var score = scoreLoader?.RuntimeScore;

            if (score == null) {
                return;
            }

            var notes = score.Notes;

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

            var scalingResponder = theaterDays.FindSingleElement<MltdStageScalingResponder>();
            if (scalingResponder == null) {
                throw new InvalidOperationException();
            }

            var now = (float)syncTimer.CurrentTime.TotalSeconds;

            var tapPointsConfig = ConfigurationStore.Get<TapPointsConfig>();
            var notesLayerConfig = ConfigurationStore.Get<NotesLayerConfig>();
            var tapPointsLayout = tapPointsConfig.Data.Layout;
            var notesLayerLayout = notesLayerConfig.Data.Layout;
            var clientSize = theaterDays.GraphicsDevice.Viewport;

            var traceCalculator = notesLayer.AnimationCalculator;

            var animationMetrics = new NoteAnimationMetrics {
                GlobalSpeedScale = notesLayer.GlobalSpeedScale,
                Width = clientSize.Width,
                Height = clientSize.Height,
                Top = notesLayerLayout.Y.IsPercentage ? notesLayerLayout.Y.Value * clientSize.Height : notesLayerLayout.Y.Value,
                Bottom = tapPointsLayout.Y.IsPercentage ? tapPointsLayout.Y.Value * clientSize.Height : tapPointsLayout.Y.Value,
                NoteStartXRatios = tapPoints.StartXRatios,
                NoteEndXRatios = tapPoints.EndXRatios,
                TrackCount = tapPoints.EndXRatios.Length
            };

            var topYRatio = _ribbonTopYRatio;
            var bottomYRatio = _ribbonBottomYRatio;

            var ribbonEffect = theaterDays.EffectManager.Get<RibbonEffect>();

            ribbonEffect.CurrentTime = now;

            var graphics = theaterDays.GraphicsDevice;

            // Enable depth buffer to allow drawing in layers.
            // However the depth comparison always passes, and Z always decreases (see below), so the results looks like
            // "later-drawn-on-top" inside this visual element.
            // We are using Begin3DFast() here, so the states are specified in effect file (simple_texture.fx). Please
            // open that file to see which settings we are selecting.

            // Z value should be decreasing to achieve this effect:
            // when two ribbons cross, the one with the start note on the right is above the other.
            var z = ZTop;

            var smallNoteMetrics = new NoteMetrics {
                StartRadius = scalingResponder.ScaleResults.VisualNoteSmall.Start,
                EndRadius = scalingResponder.ScaleResults.VisualNoteSmall.End
            };
            var largeNoteMetrics = new NoteMetrics {
                StartRadius = scalingResponder.ScaleResults.VisualNoteLarge.Start,
                EndRadius = scalingResponder.ScaleResults.VisualNoteLarge.End
            };

            var viewProjection = _viewMatrix * _projectionMatrix;

            foreach (var note in notes) {
                OnStageStatus thisStatus, nextStatus;
                RuntimeNote nextNote;
                NoteMetrics visualNoteMetrics;
                RibbonMeshCreateParams rmcp;

                if (note.HasNextHold()) {
                    thisStatus = NoteAnimationHelper.GetOnStageStatusOf(note, now, animationMetrics);

                    if (thisStatus == OnStageStatus.Incoming) {
                        continue;
                    }

                    nextNote = note.NextHold;
                    nextStatus = NoteAnimationHelper.GetOnStageStatusOf(nextNote, now, animationMetrics);

                    if (AreTwoNotesOnTheSameSide(thisStatus, nextStatus)) {
                        continue;
                    }

                    var firstHoldInGroup = note;
                    while (firstHoldInGroup.HasPrevHold()) {
                        firstHoldInGroup = firstHoldInGroup.PrevHold;
                    }
                    visualNoteMetrics = firstHoldInGroup.IsFlick() || firstHoldInGroup.Size == NoteSize.Large ? largeNoteMetrics : smallNoteMetrics;

                    var ribbonParams = traceCalculator.GetHoldRibbonParameters(note, nextNote, now, visualNoteMetrics, animationMetrics);

                    if (ribbonParams.Visible) {
                        rmcp = new RibbonMeshCreateParams(graphics, SliceCount, topYRatio, bottomYRatio, z, LayerDepth, new[] { ribbonParams }, traceCalculator, now, new[] { (note, note.NextHold) }, visualNoteMetrics, animationMetrics);
                        using (var mesh = new RibbonMesh(rmcp)) {
                            graphics.DrawRibbon(mesh, ribbonEffect, viewProjection, RibbonMaterial, _ribbonTexture);
                        }

                        z -= LayerDepth;
                    }
                }

                if (note.HasNextFlick()) {
                    thisStatus = NoteAnimationHelper.GetOnStageStatusOf(note, now, animationMetrics);

                    if (thisStatus == OnStageStatus.Incoming) {
                        continue;
                    }

                    nextNote = note.NextFlick;
                    nextStatus = NoteAnimationHelper.GetOnStageStatusOf(nextNote, now, animationMetrics);

                    if (thisStatus == OnStageStatus.Visible && note.HasPrevFlick()) {
                        // We have already drawn this flick group.
                        continue;
                    }

                    if (AreTwoNotesOnTheSameSide(thisStatus, nextStatus)) {
                        continue;
                    }

                    var firstFlickInGroup = note;
                    while (firstFlickInGroup.HasPrevFlick()) {
                        firstFlickInGroup = firstFlickInGroup.PrevFlick;
                    }
                    visualNoteMetrics = firstFlickInGroup.IsFlick() || firstFlickInGroup.Size == NoteSize.Large ? largeNoteMetrics : smallNoteMetrics;

                    if (nextStatus == OnStageStatus.Visible && nextNote.HasNextFlick()) {
                        var nextFlicks = new List<RuntimeNote>();
                        nextFlicks.Add(nextNote);

                        do {
                            nextNote = nextNote.NextFlick;
                            nextStatus = NoteAnimationHelper.GetOnStageStatusOf(nextNote, now, animationMetrics);
                            nextFlicks.Add(nextNote);
                        } while (nextNote.HasNextFlick() && nextStatus > OnStageStatus.Incoming);

                        var flickNotePairCount = nextFlicks.Count;
                        var ribbonParamArray = new RibbonParameters[flickNotePairCount];
                        var flickNotePairs = new(RuntimeNote, RuntimeNote)[flickNotePairCount];
                        for (var i = 0; i < flickNotePairCount; ++i) {
                            if (i == 0) {
                                flickNotePairs[i] = (note, nextFlicks[0]);
                                ribbonParamArray[i] = traceCalculator.GetFlickRibbonParameters(note, nextFlicks[0], now, visualNoteMetrics, animationMetrics);
                            } else {
                                flickNotePairs[i] = (nextFlicks[i - 1], nextFlicks[i]);
                                ribbonParamArray[i] = traceCalculator.GetFlickRibbonParameters(nextFlicks[i - 1], nextFlicks[i], now, visualNoteMetrics, animationMetrics);
                            }
                        }

                        if (ribbonParamArray.Any(rp => rp.Visible)) {
                            rmcp = new RibbonMeshCreateParams(graphics, SliceCount, topYRatio, bottomYRatio, z, LayerDepth, ribbonParamArray, traceCalculator, now, flickNotePairs, visualNoteMetrics, animationMetrics);
                            using (var mesh = new RibbonMesh(rmcp)) {
                                graphics.DrawRibbon(mesh, ribbonEffect, viewProjection, RibbonMaterial, _ribbonTexture);
                            }

                            z -= LayerDepth;
                        }
                    } else {
                        var ribbonParams = traceCalculator.GetSlideRibbonParameters(note, nextNote, now, visualNoteMetrics, animationMetrics);

                        if (ribbonParams.Visible) {
                            rmcp = new RibbonMeshCreateParams(graphics, SliceCount, topYRatio, bottomYRatio, z, LayerDepth, new[] { ribbonParams }, traceCalculator, now, new[] { (note, note.NextSlide) }, visualNoteMetrics, animationMetrics);
                            using (var mesh = new RibbonMesh(rmcp)) {
                                graphics.DrawRibbon(mesh, ribbonEffect, viewProjection, RibbonMaterial, _ribbonTexture);
                            }

                            z -= LayerDepth;
                        }
                    }
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

                    if (AreTwoNotesOnTheSameSide(thisStatus, nextStatus)) {
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

                        if (ribbonParamArray.Any(rp => rp.Visible)) {
                            rmcp = new RibbonMeshCreateParams(graphics, SliceCount, topYRatio, bottomYRatio, z, LayerDepth, ribbonParamArray, traceCalculator, now, slideNotePairs, visualNoteMetrics, animationMetrics);
                            using (var mesh = new RibbonMesh(rmcp)) {
                                graphics.DrawRibbon(mesh, ribbonEffect, viewProjection, RibbonMaterial, _ribbonTexture);
                            }

                            z -= LayerDepth;
                        }
                    } else {
                        var ribbonParams = traceCalculator.GetSlideRibbonParameters(note, nextNote, now, visualNoteMetrics, animationMetrics);

                        if (ribbonParams.Visible) {
                            rmcp = new RibbonMeshCreateParams(graphics, SliceCount, topYRatio, bottomYRatio, z, LayerDepth, new[] { ribbonParams }, traceCalculator, now, new[] { (note, note.NextSlide) }, visualNoteMetrics, animationMetrics);
                            using (var mesh = new RibbonMesh(rmcp)) {
                                graphics.DrawRibbon(mesh, ribbonEffect, viewProjection, RibbonMaterial, _ribbonTexture);
                            }

                            z -= LayerDepth;
                        }
                    }
                }
            }
        }

        protected override void OnLoadContents() {
            base.OnLoadContents();

            var theaterDays = Game.ToBaseGame();

            var config = ConfigurationStore.Get<RibbonConfig>();
            var gamingArea = theaterDays.FindSingleElement<MltdStage>();

            if (gamingArea == null) {
                throw new InvalidOperationException();
            }

            var viewport = theaterDays.GraphicsDevice.Viewport;
            var clientSize = new Vector2(viewport.Width, viewport.Height);
            var centerPoint = new Vector2(clientSize.X / 2, clientSize.Y / 2);

            _viewMatrix = Matrix.CreateLookAt(new Vector3(centerPoint.X, centerPoint.Y, CameraZ), new Vector3(centerPoint.X, centerPoint.Y, 0), -Vector3.UnitY);
            // TODO: X axis - what the hell?
            _projectionMatrix = Matrix.CreateOrthographicOffCenter(clientSize.X / 2, -clientSize.X / 2, -clientSize.Y / 2, clientSize.Y / 2, 0.1f, ViewFrustrumDepth);

            _ribbonTexture = ContentHelper.LoadTexture(theaterDays.GraphicsDevice, config.Data.Image.FileName);

            theaterDays.EffectManager.RegisterSingleton<RibbonEffect>(@"Contents/res/fx/ribbon.fx");

            var textureDesc = _ribbonTexture.Bounds;
            var textureBlankEdges = config.Data.Image.BlankEdge;
            _ribbonTopYRatio = textureBlankEdges.Top / textureDesc.Height;
            _ribbonBottomYRatio = (textureDesc.Height - textureBlankEdges.Bottom) / textureDesc.Height;
        }

        protected override void OnUnloadContents() {
            _ribbonTexture?.Dispose();

            base.OnUnloadContents();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool AreTwoNotesOnTheSameSide(OnStageStatus thisStatus, OnStageStatus nextStatus) {
            return (int)thisStatus * (int)nextStatus > 0;
        }

        private static readonly Material RibbonMaterial = new Material {
            Diffuse = Vector4.One
        };

        private static readonly int SliceCount = 48;
        private static readonly float ZTop = 100f;
        private static readonly float LayerDepth = 1f;
        private static readonly float CameraZ = ZTop + LayerDepth;
        private static readonly float ViewFrustrumDepth = 100f;

        private Matrix _viewMatrix;
        private Matrix _projectionMatrix;

        private Texture2D _ribbonTexture;

        private float _ribbonTopYRatio;
        private float _ribbonBottomYRatio;

    }
}
