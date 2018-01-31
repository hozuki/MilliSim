using System;
using System.Diagnostics;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Extension.Components.ScoreComponents.Configuration;
using OpenMLTD.MilliSim.Extension.Components.ScoreComponents.Gaming;
using OpenMLTD.MilliSim.Extension.Components.ScoreComponents.Overlays;
using OpenMLTD.MilliSim.Extension.Components.ScoreComponents.Overlays.Combo;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Foundation.Extensions;
using OpenMLTD.MilliSim.Graphics;

namespace OpenMLTD.MilliSim.Extension.Components.ScoreComponents {
    [MilliSimPlugin(typeof(IComponentFactory))]
    public sealed class MltdStageFactory : ComponentFactory {

        public override string PluginID => "plugin.component_factory.mltd_stage";

        public override string PluginName => "MltdStage Component Factory";

        public override string PluginDescription => "MltdStage Component Factory";

        public override string PluginAuthor => "OpenMLTD";

        public override Version PluginVersion => MyVersion;

        public override IComponent CreateComponent(GameBase game, IComponentContainer parent) {
            Trace.Assert(parent is IVisualContainer);

            var mltdStage = new MltdStage((IVisualContainer)parent);
            var store = game.ConfigurationStore;

            mltdStage.CreateAndAdd<MltdStageScalingResponder>();
            mltdStage.CreateAndAdd<ScoreLoader>();
            mltdStage.CreateAndAdd<NoteReactor>();

            var slideMotionConfig = store.Get<SlideMotionConfig>();

            if (slideMotionConfig.Data.Icon != SlideMotionConfig.SlideMotionIcon.None && slideMotionConfig.Data.Position == SlideMotionConfig.SlideMotionPosition.Below) {
                mltdStage.CreateAndAdd<SlideMotion>();
            }
            mltdStage.CreateAndAdd<RibbonsLayer>();
            if (slideMotionConfig.Data.Icon != SlideMotionConfig.SlideMotionIcon.None && slideMotionConfig.Data.Position == SlideMotionConfig.SlideMotionPosition.Above) {
                mltdStage.CreateAndAdd<SlideMotion>();
            }
            mltdStage.CreateAndAdd<TapPointsMergingAnimation>();
            mltdStage.CreateAndAdd<NotesLayer>();
            mltdStage.CreateAndAdd<TapPoints>();
            mltdStage.CreateAndAdd<HitRankAnimation>();

            var comboDisplay = mltdStage.CreateAndAdd<ComboDisplay>();
            comboDisplay.CreateAndAdd<ComboAura>();
            comboDisplay.CreateAndAdd<ComboText>();
            comboDisplay.CreateAndAdd<ComboNumbers>();

            mltdStage.CreateAndAdd<AvatarDisplay>();
            mltdStage.CreateAndAdd<SongTitle>();

            return mltdStage;
        }

        private static readonly Version MyVersion = new Version(1, 0, 0, 0);

    }
}
