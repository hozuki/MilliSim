using System;
using System.Diagnostics;
using System.IO;
using MonoGame.Extended.VideoPlayback;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Extension.Components.CoreComponents.Configuration;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Foundation.Extending;
using OpenMLTD.MilliSim.Graphics;

namespace OpenMLTD.MilliSim.Extension.Components.CoreComponents {
    /// <inheritdoc />
    /// <summary>
    /// A <see cref="BaseGameComponentFactory"/> that creates <see cref="BackgroundVideo"/>.
    /// </summary>
    [MilliSimPlugin(typeof(IBaseGameComponentFactory))]
    public sealed class BackgroundVideoFactory : BaseGameComponentFactory {

        public override string PluginID => "plugin.component_factory.background_video";

        public override string PluginName => "BackgroundVideo Component Factory";

        public override string PluginDescription => "BackgroundVideo Component Factory";

        public override string PluginAuthor => "OpenMLTD";

        public override Version PluginVersion => MyVersion;

        public override IBaseGameComponent CreateComponent(BaseGame game, IBaseGameComponentContainer parent) {
            var config = game.ConfigurationStore.Get<BackgroundVideoConfig>();
            if (!string.IsNullOrEmpty(config.Data.BackgroundVideo) && File.Exists(config.Data.BackgroundVideo)) {
                Trace.Assert(parent is IVisualContainer);

                InitializeFFmpeg();

                var video = new BackgroundVideo(game, (IVisualContainer)parent);

                video.Load(config.Data.BackgroundVideo);
                video.Volume = config.Data.BackgroundVideoVolume.Value;

                return video;
            } else {
                return null;
            }
        }

        private static void InitializeFFmpeg() {
            if (_isFFmpegInitialized) {
                return;
            }

            FFmpegBinariesHelper.InitializeFFmpeg("x86", "x64");
            _isFFmpegInitialized = true;
        }

        private static readonly Version MyVersion = new Version(1, 0, 0, 0);

        private static bool _isFFmpegInitialized;

    }
}
