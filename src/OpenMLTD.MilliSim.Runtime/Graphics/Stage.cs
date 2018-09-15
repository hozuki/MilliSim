using JetBrains.Annotations;
using OpenMLTD.MilliSim.Configuration;
using OpenMLTD.MilliSim.Foundation;

namespace OpenMLTD.MilliSim.Graphics {
    // Why BufferedVisualContainer but not VisualContainer?
    // Its children may contain `IBufferedVisual`s, which requires switching render targets.
    // If Stage inherits from VisualContainer, its draw calls are directly send to GraphicsDevice.
    // In this case, when the first child of Stage, which has a parent that doesn't create a render target, ends a draw call,
    // the GraphicsDevice will eventually call SetRenderTarget(null), which clears the screen, all all contents prior to
    // the child's draw call. The result is you will see only the content of the last IBufferedVisual drawn, and the contents
    // that only directly draw on the screen after that IBufferedVisual.
    // For more information, see GraphicsDeviceExtensions.CreateRenderTarget().
    /// <summary>
    /// The main stage.
    /// </summary>
    public sealed class Stage : BufferedVisualContainer {

        /// <summary>
        /// Creates a new <see cref="Stage"/> instance.
        /// </summary>
        /// <param name="game">The base game.</param>
        /// <param name="configurationStore">The <see cref="BaseConfigurationStore"/> for this <see cref="Stage"/>.</param>
        public Stage([NotNull] BaseGame game, [NotNull] BaseConfigurationStore configurationStore)
            // ReSharper disable once AssignNullToNotNullAttribute
            : base(game, null) {
            ConfigurationStore = configurationStore;
        }

    }
}
