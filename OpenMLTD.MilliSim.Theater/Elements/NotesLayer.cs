using System.IO;
using OpenMLTD.MilliSim.Core;
using OpenMLTD.MilliSim.Foundation;
using OpenMLTD.MilliSim.Graphics;
using OpenMLTD.MilliSim.Graphics.Drawing;
using OpenMLTD.MilliSim.Graphics.Drawing.Direct2D.Advanced;
using OpenMLTD.MilliSim.Graphics.Extensions;
using OpenMLTD.MilliSim.Theater.Extensions;

namespace OpenMLTD.MilliSim.Theater.Elements {
    public class NotesLayer : BufferedElement2D {

        public NotesLayer(GameBase game)
            : base(game) {
        }

        protected override void OnDrawBuffer(GameTime gameTime, RenderContext context) {
            base.OnDrawBuffer(gameTime, context);

            var settings = Program.Settings;
            var scaledSize = settings.Scaling.Note;

            context.Begin2D();
            if (_noteImages?[0] != null) {
                var strip = _noteImages[0];
                context.DrawImageStrip(strip, 1, 130, 340, scaledSize.Width, scaledSize.Height);
            }
            context.End2D();
        }

        protected override void OnGotContext(RenderContext context) {
            base.OnGotContext(context);

            var settings = Program.Settings;
            var theaterDays = Game.AsTheaterDays();
            var debugOverlay = theaterDays.GetDebugOverlay();

            if (settings.Images.Notes.Length == 0) {
                if (debugOverlay != null) {
                    debugOverlay.AddLine("WARNING: default notes image strip is not specified.");
                }
            } else {
                _noteImages = new D2DImageStrip[settings.Images.Notes.Length];

                for (var i = 0; i < settings.Images.Notes.Length; ++i) {
                    var notesImageEntry = settings.Images.Notes[i];
                    if (notesImageEntry == null) {
                        continue;
                    }

                    if (notesImageEntry.File == null || !File.Exists(notesImageEntry.File)) {
                        if (i == 0) {
                            if (debugOverlay != null) {
                                debugOverlay.AddLine($"WARNING: default notes image strip <{notesImageEntry.File ?? string.Empty}> is not found.");
                            }
                        } else {
                            if (debugOverlay != null) {
                                debugOverlay.AddLine($"WARNING: notes image strip <{notesImageEntry.File ?? string.Empty}> is not found, falling back to default.");
                            }
                        }
                        continue;
                    }

                    var imageStrip = Direct2DHelper.LoadImageStrip(context, notesImageEntry.File, notesImageEntry.Count, notesImageEntry.Orientation);
                    _noteImages[i] = imageStrip;
                }
            }

            Opacity = settings.UI.NotesLayer.Opacity;
        }

        protected override void OnLostContext(RenderContext context) {
            base.OnLostContext(context);

            if (_noteImages != null) {
                foreach (var noteImage in _noteImages) {
                    noteImage?.Dispose();
                }
            }
            _noteImages = null;
        }

        private D2DImageStrip[] _noteImages;

    }
}
