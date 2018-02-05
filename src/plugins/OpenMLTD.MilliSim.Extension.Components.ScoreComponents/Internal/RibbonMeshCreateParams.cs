using JetBrains.Annotations;
using Microsoft.Xna.Framework.Graphics;
using OpenMLTD.MilliSim.Contributed.Scores.Animation;
using OpenMLTD.MilliSim.Contributed.Scores.Extending;
using OpenMLTD.MilliSim.Contributed.Scores.Runtime;

namespace OpenMLTD.MilliSim.Extension.Components.ScoreComponents.Internal {
    internal struct RibbonMeshCreateParams {

        internal RibbonMeshCreateParams([NotNull] GraphicsDevice device, int slice, float textureTopYRatio, float textureBottomYRatio, float z,
            float layerDepth, RibbonParameters[] rps, INoteAnimationCalculator animationCalculator, float now,
            [NotNull] (RuntimeNote Start, RuntimeNote End)[] notePairs, NoteMetrics visualNoteMetrics, NoteAnimationMetrics animationMetrics) {
            Device = device;
            Slice = slice;
            TextureTopYRatio = textureTopYRatio;
            TextureBottomYRatio = textureBottomYRatio;
            Z = z;
            LayerDepth = layerDepth;
            RibbonParameters = rps;
            AnimationCalculator = animationCalculator;
            Now = now;
            NotePairs = notePairs;
            VisualNoteMetrics = visualNoteMetrics;
            AnimationMetrics = animationMetrics;
        }

        internal GraphicsDevice Device { get; }

        internal int Slice { get; }

        internal float TextureTopYRatio { get; }

        internal float TextureBottomYRatio { get; }

        internal float Z { get; }

        internal float LayerDepth { get; }

        internal RibbonParameters[] RibbonParameters { get; }

        internal INoteAnimationCalculator AnimationCalculator { get; }

        internal float Now { get; }

        internal (RuntimeNote Start, RuntimeNote End)[] NotePairs { get; }

        internal NoteMetrics VisualNoteMetrics { get; }

        internal NoteAnimationMetrics AnimationMetrics { get; }

    }
}
