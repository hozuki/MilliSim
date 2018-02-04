using System;
using System.Diagnostics;
using OpenMLTD.MilliSim.Extension.Components.ScoreComponents.Configuration;
using OpenMLTD.MilliSim.Extension.Components.ScoreComponents.Extensions;
using OpenMLTD.MilliSim.Extension.Components.ScoreComponents.Gaming;
using OpenMLTD.MilliSim.Extension.Components.ScoreComponents.Overlays;
using OpenMLTD.MilliSim.Extension.Components.ScoreComponents.Overlays.Combo;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Graphics;
using OpenMLTD.MilliSim.Plugin;

namespace OpenMLTD.MilliSim.Extension.Components.ScoreComponents {
    [MilliSimPlugin(typeof(IBaseGameComponentFactory))]
    public sealed class MltdStageFactory : BaseGameComponentFactory {

        public override string PluginID => "plugin.component_factory.mltd_stage";

        public override string PluginName => "MltdStage Component Factory";

        public override string PluginDescription => "MltdStage Component Factory";

        public override string PluginAuthor => "OpenMLTD";

        public override Version PluginVersion => MyVersion;

        public override IBaseGameComponent CreateComponent(BaseGame game, IBaseGameComponentContainer parent) {
            Trace.Assert(parent is IVisualContainer);

            var mltdStage = new MltdStage(game, (IVisualContainer)parent);
            var store = game.ConfigurationStore;

            mltdStage.CreateAndAdd<MltdStageScalingResponder>(game);
            mltdStage.CreateAndAdd<ScoreLoader>(game);
            mltdStage.CreateAndAdd<NoteReactor>(game);

            var slideMotionConfig = store.Get<SlideMotionConfig>();

            if (slideMotionConfig.Data.Icon != SlideMotionConfig.SlideMotionIcon.None && slideMotionConfig.Data.Position == SlideMotionConfig.SlideMotionPosition.Below) {
                mltdStage.CreateAndAdd<SlideMotion>(game);
            }
            mltdStage.CreateAndAdd<RibbonsLayer>(game);
            if (slideMotionConfig.Data.Icon != SlideMotionConfig.SlideMotionIcon.None && slideMotionConfig.Data.Position == SlideMotionConfig.SlideMotionPosition.Above) {
                mltdStage.CreateAndAdd<SlideMotion>(game);
            }
            mltdStage.CreateAndAdd<TapPointsMergingAnimation>(game);
            mltdStage.CreateAndAdd<NotesLayer>(game);
            mltdStage.CreateAndAdd<TapPoints>(game);
            mltdStage.CreateAndAdd<HitRankAnimation>(game);

            var comboDisplay = mltdStage.CreateAndAdd<ComboDisplay>(game);
            comboDisplay.CreateAndAdd<ComboAura>(game);
            comboDisplay.CreateAndAdd<ComboText>(game);
            comboDisplay.CreateAndAdd<ComboNumbers>(game);

            mltdStage.CreateAndAdd<AvatarDisplay>(game);
            mltdStage.CreateAndAdd<SongTitle>(game);

            return mltdStage;
        }

        private static readonly Version MyVersion = new Version(1, 0, 0, 0);

    }
}
