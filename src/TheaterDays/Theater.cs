using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpenMLTD.MilliSim.Configuration;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Extension.Components.CoreComponents;
using OpenMLTD.MilliSim.Extension.Components.ScoreComponents.Configuration;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Foundation.Extending;
using OpenMLTD.MilliSim.Foundation.Extensions;
using OpenMLTD.MilliSim.Globalization;
using OpenMLTD.MilliSim.Graphics;
using OpenMLTD.TheaterDays.Configuration;
using OpenMLTD.TheaterDays.Interactive;
using OpenMLTD.TheaterDays.Subsystems.Bvs;
using OpenMLTD.TheaterDays.Subsystems.Plugin;

namespace OpenMLTD.TheaterDays {
    public sealed partial class Theater : BaseGame {

        private Theater([NotNull] Options startupOptions, [NotNull] BasePluginManager pluginManager, [NotNull] ConfigurationStore configurationStore, [NotNull] CultureSpecificInfo cultureSpecificInfo)
            : base("Contents", pluginManager) {
            StartupOptions = startupOptions;

            ConfigurationStore = configurationStore;
            CultureSpecificInfo = cultureSpecificInfo;

            // For Direct3D, the method must be called here, not in Initialize().
            // When called in Initialize(), DesktopGL behaves fine, but Direct3D fails to resize the window.
            ApplyConfiguration();
        }

        [NotNull]
        public Options StartupOptions { get; }

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

            var editorServerPort = StartupOptions.EditorServerPort;
            var editorServerUri = StartupOptions.EditorServerUri;

            if (editorServerPort > 0 || !string.IsNullOrWhiteSpace(editorServerUri)) {

                _communication = new TDCommunication(this);

                Uri edServerUri;

                if (string.IsNullOrWhiteSpace(editorServerUri)) {
                    edServerUri = new Uri($"http://localhost:{editorServerPort}/");
                } else {
                    var b = Uri.TryCreate(editorServerUri, UriKind.RelativeOrAbsolute, out edServerUri);

                    if (!b) {
                        GameLog.Error("Invalid URI format (in editor_server_uri command line param): {0}", editorServerUri);
                    }
                }

                _communication.EditorServerUri = edServerUri;

                int simulatorServerPort;

#if DEBUG
                simulatorServerPort = StartupOptions.BvspPort > 0 ? StartupOptions.BvspPort : 0;
#else
                simulatorServerPort = 0;
#endif

                _communication.Server.Start(simulatorServerPort);

                // No await
                _communication.Client.SendLaunchedNotification();
            }
        }

        protected override void UnloadContent() {
            UnsubscribeComponentEvents();

            if (_communication != null) {
                _communication.Client.SendSimExitedNotification().Wait(TimeSpan.FromSeconds(2));

                _communication.Server.Stop();

                _communication.Dispose();
            }

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

            string songTitle;
            if (ConfigurationStore.TryGetValue<ScoreLoaderConfig>(out var scoreLoaderConfig)) {
                songTitle = scoreLoaderConfig.Data.Title;
            } else {
                songTitle = null;
            }

            var appCodeName = ApplicationHelper.CodeName;
            var windowTitle = config.Data.Window.Title;

            if (!string.IsNullOrWhiteSpace(songTitle)) {
                windowTitle = songTitle + " - " + windowTitle;
            }

            if (!string.IsNullOrWhiteSpace(appCodeName)) {
                windowTitle = windowTitle + " (\"" + appCodeName + "\")";
            }

            Window.Title = windowTitle;
        }

        private void SubscribeComponentEvents() {
            // Subscribe to music playback stopped event.
            var syncTimer = this.FindSingleElement<SyncTimer>();

            if (syncTimer != null) {
                syncTimer.StateChanged += SyncTimer_StateChanged;
            }
        }

        private void UnsubscribeComponentEvents() {
            // Subscribe to music playback stopped event.
            var syncTimer = this.FindSingleElement<SyncTimer>();

            if (syncTimer != null) {
                syncTimer.StateChanged -= SyncTimer_StateChanged;
            }
        }

        private Stage _stage;
        [CanBeNull]
        private TDCommunication _communication;

    }
}
