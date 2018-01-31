using System;
using System.Diagnostics;
using System.IO;
using MonoGame.Extended.VideoPlayback;
using OpenMLTD.MilliSim.Extension.Components.CoreComponents.Configuration;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Graphics;
using OpenMLTD.MilliSim.Plugin;

namespace OpenMLTD.MilliSim.Extension.Components.CoreComponents {
    [MilliSimPlugin(typeof(IBaseGameComponentFactory))]
    public sealed class BackgroundVideoFactory : BaseGameComponentFactory {

        static BackgroundVideoFactory() {
            FFmpegBinariesHelper.InitializeFFmpeg("x86", "x64");
        }

        public override string PluginID => "plugin.component_factory.background_video";

        public override string PluginName => "BackgroundVideo Component Factory";

        public override string PluginDescription => "BackgroundVideo Component Factory";

        public override string PluginAuthor => "OpenMLTD";

        public override Version PluginVersion => MyVersion;

        public override IBaseGameComponent CreateComponent(BaseGame game, IBaseGameComponentContainer parent) {
            var config = game.ConfigurationStore.Get<BackgroundVideoConfig>();
            if (!string.IsNullOrEmpty(config.Data.BackgroundVideo) && File.Exists(config.Data.BackgroundVideo)) {
                Trace.Assert(parent is IVisualContainer);

                var video = new BackgroundVideo(game, (IVisualContainer)parent);

                video.Load(config.Data.BackgroundVideo);
                video.Volume = config.Data.BackgroundVideoVolume.Value;

                return video;
            } else {
                return null;
            }
        }

        private static readonly Version MyVersion = new Version(1, 0, 0, 0);

    }
}
