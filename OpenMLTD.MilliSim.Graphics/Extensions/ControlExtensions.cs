using System;
using System.Drawing;
using System.Windows.Forms;
using JetBrains.Annotations;

namespace OpenMLTD.MilliSim.Graphics.Extensions {
    public static class ControlExtensions {

        public static void CenterToParent([NotNull] this Control control) {
            if (control == null) {
                throw new ArgumentNullException(nameof(control));
            }
            var parent = control.Parent;
            if (parent == null) {
                control.CenterToScreen();
            } else {
                control.CenterTo(parent);
            }
        }

        public static void CenterTo([NotNull] this Control control, [NotNull] Control reference) {
            if (control == null) {
                throw new ArgumentNullException(nameof(control));
            }
            if (reference == null) {
                throw new ArgumentNullException(nameof(reference));
            }
            var controlSize = control.Size;
            var referenceSize = reference.Size;
            var referenceLocation = reference.PointToScreen(Point.Empty);

            var relSize = new Size((referenceSize.Width - controlSize.Width) / 2, (referenceSize.Height - controlSize.Height) / 2);
            var absPos = referenceLocation + relSize;

            control.Location = absPos;
        }

        public static void CenterToScreen([NotNull] this Control control) {
            if (control == null) {
                throw new ArgumentNullException(nameof(control));
            }
            var workingArea = Screen.GetWorkingArea(control);
            var left = (workingArea.Width - control.Width) / 2;
            var top = (workingArea.Height - control.Height) / 2;
            control.Location = new Point(left, top);
        }

    }
}
