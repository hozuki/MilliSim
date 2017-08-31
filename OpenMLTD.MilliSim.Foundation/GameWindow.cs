using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace OpenMLTD.MilliSim.Foundation {
    public class GameWindow : Form {

        public GameWindow(GameBase game) {
            Game = game;

            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.Opaque | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer, false);

            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            KeyPreview = true;

            CheckForIllegalCrossThreadCalls = false;

            InitializeComponent();
        }

        public GameBase Game { get; }

        public event EventHandler<EventArgs> StageReady;

        internal void RaiseStageReady(EventArgs e) {
            StageReady?.Invoke(this, e);
        }

        internal void RaiseStageReadyAsync(EventArgs e) {
            StageReady?.BeginInvoke(this, e, null, null);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly IContainer _components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing) {
                _components?.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.SuspendLayout();
            this.AutoScaleDimensions = new SizeF(96F, 96F);
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.ClientSize = new Size(284, 261);
            this.Name = "GameWindow";
            this.Text = Game.Title;
            this.ResumeLayout(false);
        }

        #endregion

    }
}
