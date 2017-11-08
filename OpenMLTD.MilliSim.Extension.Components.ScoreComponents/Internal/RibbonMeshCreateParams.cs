using OpenMLTD.MilliSim.Core.Entities.Runtime;
using OpenMLTD.MilliSim.Theater.Animation;
using OpenMLTD.MilliSim.Theater.Animation.Extending;
using SharpDX.Direct3D11;

namespace OpenMLTD.MilliSim.Extension.Components.ScoreComponents.Internal {
    internal struct RibbonMeshCreateParams {

        internal RibbonMeshCreateParams(Device device, int slice, float textureTopYRatio, float textureBottomYRatio, float z, float layerDepth, RibbonParameters[] rps, INoteTraceCalculator traceCalculator, double now, (RuntimeNote Start, RuntimeNote End)[] notePairs, NoteMetrics visualNoteMetrics, NoteAnimationMetrics animationMetrics) {
            Device = device;
            Slice = slice;
            TextureTopYRatio = textureTopYRatio;
            TextureBottomYRatio = textureBottomYRatio;
            Z = z;
            LayerDepth = layerDepth;
            RibbonParameters = rps;
            TraceCalculator = traceCalculator;
            Now = now;
            NotePairs = notePairs;
            VisualNoteMetrics = visualNoteMetrics;
            AnimationMetrics = animationMetrics;
        }

        internal Device Device { get; }

        internal int Slice { get; }

        internal float TextureTopYRatio { get; }

        internal float TextureBottomYRatio { get; }

        internal float Z { get; }

        internal float LayerDepth { get; }

        internal RibbonParameters[] RibbonParameters { get; }

        internal INoteTraceCalculator TraceCalculator { get; }

        internal double Now { get; }

        internal (RuntimeNote Start, RuntimeNote End)[] NotePairs { get; }

        internal NoteMetrics VisualNoteMetrics { get; }

        internal NoteAnimationMetrics AnimationMetrics { get; }

    }
}
