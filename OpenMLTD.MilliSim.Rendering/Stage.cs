using System.Collections.Generic;
using JetBrains.Annotations;
using OpenMLTD.MilliSim.Core;

namespace OpenMLTD.MilliSim.Rendering {
    public sealed class Stage {

        public Stage([NotNull, ItemNotNull] IReadOnlyList<VisualElement> visualElements) {
            VisualElements = visualElements;
        }

        [NotNull, ItemNotNull]
        public IReadOnlyList<VisualElement> VisualElements { get; }

        public void Draw([NotNull] GameTime gameTime, [NotNull] ControlStageRenderer renderer) {
            renderer.Draw(VisualElements, gameTime);
        }

    }
}
