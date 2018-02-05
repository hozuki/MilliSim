using System;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using OpenMLTD.MilliSim.Extension.Components.CoreComponents;
using OpenMLTD.MilliSim.Extension.Components.ScoreComponents.Configuration;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Foundation.Extensions;
using OpenMLTD.MilliSim.Graphics;

namespace OpenMLTD.MilliSim.Extension.Components.ScoreComponents.Overlays {
    public class SongTitle : OutlinedTextOverlay {

        public SongTitle([NotNull]BaseGame game, [NotNull] IVisualContainer parent)
            : base(game, parent) {
        }

        protected override void OnLoadContents() {
            base.OnLoadContents();

            var viewport = Game.GraphicsDevice.Viewport;
            var config = ConfigurationStore.Get<SongTitleConfig>();
            var layout = config.Data.Layout;
            var x = layout.X.IsPercentage ? viewport.Width * layout.X.Value : layout.X.Value;
            var y = layout.Y.IsPercentage ? viewport.Height * layout.Y.Value : layout.Y.Value;

            Location = new Vector2(x, y);
        }

        protected override void OnUpdate(GameTime gameTime) {
            base.OnUpdate(gameTime);

            var syncTimer = Game.ToBaseGame().FindSingleElement<SyncTimer>();
            if (syncTimer == null) {
                throw new InvalidOperationException();
            }

            var now = syncTimer.CurrentTime.TotalSeconds;

            var config = ConfigurationStore.Get<SongTitleConfig>();
            var appearStages = config.Data.Animation.Appear;
            var reappearStages = config.Data.Animation.Reappear;

            var s1t1 = appearStages.Enter + appearStages.FadeIn;
            var s1t2 = s1t1 + appearStages.Hold;
            var s1t3 = s1t2 + appearStages.FadeOut;
            var s2t1 = reappearStages.Enter + reappearStages.FadeIn;
            var s2t2 = s2t1 + reappearStages.Hold;
            var s2t3 = s2t2 + reappearStages.FadeOut;

            if (!(appearStages.Enter <= now && now <= s1t3) && !(reappearStages.Enter <= now && now <= s2t3)) {
                Opacity = 0;
                return;
            }

            float opacity = 0;
            if (!appearStages.FadeIn.Equals(0) && now <= s1t1) {
                opacity = (float)(now - appearStages.Enter) / (float)appearStages.FadeIn;
            } else if (!appearStages.Hold.Equals(0) && now <= s1t2) {
                opacity = 1;
            } else if (!appearStages.FadeOut.Equals(0) && now <= s1t3) {
                opacity = 1 - (float)(now - s1t2) / (float)appearStages.FadeOut;
            } else if (!reappearStages.FadeIn.Equals(0) && now <= s2t1) {
                opacity = (float)(now - reappearStages.Enter) / (float)reappearStages.FadeIn;
            } else if (!reappearStages.Hold.Equals(0) && now <= s2t2) {
                opacity = 1;
            } else if (!reappearStages.FadeOut.Equals(0) && now <= s2t3) {
                opacity = 1 - (float)(now - s2t2) / (float)reappearStages.FadeOut;
            }

            Opacity = opacity;
        }

        protected override void OnInitialize() {
            base.OnInitialize();

            var scoreLoaderConfig = ConfigurationStore.Get<ScoreLoaderConfig>();
            var songTitleConfig = ConfigurationStore.Get<SongTitleConfig>();
            Text = scoreLoaderConfig.Data.Title;
            FontSize = songTitleConfig.Data.FontSize;
            OutlineThickness = songTitleConfig.Data.TextStrokeWidth;

            Opacity = 0;
        }

        protected override string GetFontFilePath() {
            var config = ConfigurationStore.Get<SongTitleConfig>();
            return config.Data.FontFile;
        }

    }
}
