using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using OpenMLTD.MilliSim.Configuration;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Globalization;
using OpenMLTD.MilliSim.Graphics;
using OpenMLTD.MilliSim.Plugin;
using OpenMLTD.TheaterDays.Configuration;
using OpenMLTD.TheaterDays.Interactive;
using OpenMLTD.TheaterDays.Subsystems.Plugin;

namespace OpenMLTD.TheaterDays {
    internal sealed partial class Theater : BaseGame {

        internal Theater([NotNull] BasePluginManager pluginManager, [NotNull] ConfigurationStore configurationStore, [NotNull] CultureSpecificInfo cultureSpecificInfo)
            : base("Contents", pluginManager) {
            ConfigurationStore = configurationStore;
            CultureSpecificInfo = cultureSpecificInfo;
        }

        public override Stage Stage => _stage;

        [NotNull]
        public override ConfigurationStore ConfigurationStore { get; }

        public override CultureSpecificInfo CultureSpecificInfo { get; }

        protected override void Initialize() {
            ApplyConfiguration();
            AppendComponents();
            AppendExtensionComponents();

            base.Initialize();
        }

        private void ApplyConfiguration() {
            var graphics = GraphicsDeviceManager;
            var window = Window;
            var store = ConfigurationStore;
            var config = store.Get<MainAppConfig>();

            var windowSection = config.Data.Window;

            graphics.PreferredBackBufferWidth = windowSection.Width;
            graphics.PreferredBackBufferHeight = windowSection.Height;
            graphics.PreferMultiSampling = true;
            graphics.SynchronizeWithVerticalRetrace = true;

            graphics.ApplyChanges();

            window.Title = windowSection.Title;

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

        private Stage _stage;

    }
}
