using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpenMLTD.MilliSim.Configuration;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Extension.Components.CoreComponents;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Foundation.Extensions;
using OpenMLTD.MilliSim.Globalization;
using OpenMLTD.MilliSim.Graphics;
using OpenMLTD.MilliSim.Plugin;
using OpenMLTD.TheaterDays.Configuration;
using OpenMLTD.TheaterDays.Interactive;
using OpenMLTD.TheaterDays.Subsystems.Plugin;

namespace OpenMLTD.TheaterDays {
    public sealed partial class Theater : BaseGame {

        private Theater([NotNull] BasePluginManager pluginManager, [NotNull] ConfigurationStore configurationStore, [NotNull] CultureSpecificInfo cultureSpecificInfo)
            : base("Contents", pluginManager) {
            ConfigurationStore = configurationStore;
            CultureSpecificInfo = cultureSpecificInfo;

            // For Direct3D, the method must be called here, not in Initialize().
            // When called in Initialize(), DesktopGL behaves fine, but Direct3D fails to resize the window.
            ApplyConfiguration();
        }

        public override Stage Stage => _stage;

        [NotNull]
        public override ConfigurationStore ConfigurationStore { get; }

        public override CultureSpecificInfo CultureSpecificInfo { get; }

        protected override void Initialize() {
            AppendComponents();
            AppendExtensionComponents();

            CenterWindowAndSetTitle();

            base.Initialize();
        }

        protected override void LoadContent() {
            base.LoadContent();

            SubscribeComponentEvents();
        }

        protected override void UnloadContent() {
            UnsubscribeComponentEvents();

            base.UnloadContent();
        }

        private void ApplyConfiguration() {
            var graphicsManager = GraphicsDeviceManager;
            var config = ConfigurationStore.Get<MainAppConfig>();

            graphicsManager.PreferredBackBufferWidth = config.Data.Window.Width;
            graphicsManager.PreferredBackBufferHeight = config.Data.Window.Height;
            // Direct3D does not support this call
            //graphicsManager.PreferMultiSampling = true;
            graphicsManager.SynchronizeWithVerticalRetrace = true;
            graphicsManager.GraphicsProfile = GraphicsProfile.HiDef;

            IsMouseVisible = true;
        }

        private void AppendComponents() {
            var components = Components;

            var keyboardStateHandler = new KeyboardStateHandler(this);
            components.Add(keyboardStateHandler);

            keyboardStateHandler.KeyDown += KeyboardStateHandler_KeyDown;
            keyboardStateHandler.KeyUp += KeyboardStateHandler_KeyUp;
        }

        private void AppendExtensionComponents() {
            var stage = new Stage(this, ConfigurationStore);
            _stage = stage;

            Components.Add(stage);

            var pluginManager = (TheaterDaysPluginManager)PluginManager;

            var instantiatedIDList = new List<string>();

            foreach (var factoryID in pluginManager.InstancingFactoryIDs) {
                var factory = pluginManager.GetPluginByID<IBaseGameComponentFactory>(factoryID);
                var component = factory?.CreateComponent(this, stage);

                if (component != null) {
                    stage.Components.Add(component);
                    instantiatedIDList.Add(factoryID);
                }
            }

            if (instantiatedIDList.Count > 0) {
                GameLog.Debug("Instantiated component factories: {0}", string.Join(", ", instantiatedIDList));
            } else {
                GameLog.Debug("No component factory instantiated.");
            }
        }

        // For Direct3D, who fails to center the window on startup.
        private void CenterWindowAndSetTitle() {
            var windowBounds = Window.ClientBounds;
            var displayMode = GraphicsDevice.Adapter.CurrentDisplayMode;

            Window.Position = new Point((displayMode.Width - windowBounds.Width) / 2, (displayMode.Height - windowBounds.Height) / 2);

            var config = ConfigurationStore.Get<MainAppConfig>();

            Window.Title = config.Data.Window.Title;
        }

        private void SubscribeComponentEvents() {
            // Subscribe to music playback stopped event.
            var audioController = this.FindSingleElement<AudioController>();

            if (audioController?.Music != null) {
                audioController.Music.Source.PlaybackStopped += BackgroundMedia_PlaybackStopped;
            }
        }

        private void UnsubscribeComponentEvents() {
            // Subscribe to music playback stopped event.
            var audioController = this.FindSingleElement<AudioController>();

            if (audioController?.Music != null) {
                audioController.Music.Source.PlaybackStopped -= BackgroundMedia_PlaybackStopped;
            }
        }

        private Stage _stage;

    }
}
