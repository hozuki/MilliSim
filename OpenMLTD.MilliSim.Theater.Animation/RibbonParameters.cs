namespace OpenMLTD.MilliSim.Theater.Animation {
    public struct RibbonParameters {

        public RibbonParameters(float x1, float y1, float x2, float y2) {
            X1 = x1;
            Y1 = y1;
            ControlX1 = 0;
            ControlY1 = 0;
            ControlX2 = 0;
            ControlY2 = 0;
            X2 = x2;
            Y2 = y2;
            IsLine = true;
            Visible = true;
        }

        public RibbonParameters(float x1, float y1, float cx1, float cy1, float cx2, float cy2, float x2, float y2) {
            X1 = x1;
            Y1 = y1;
            ControlX1 = cx1;
            ControlY1 = cy1;
            ControlX2 = cx2;
            ControlY2 = cy2;
            X2 = x2;
            Y2 = y2;
            IsLine = false;
            Visible = true;
        }

        public RibbonParameters(float x1, float y1, float cx1, float cy1, float cx2, float cy2, float x2, float y2, bool isLine) {
            X1 = x1;
            Y1 = y1;
            ControlX1 = cx1;
            ControlY1 = cy1;
            ControlX2 = cx2;
            ControlY2 = cy2;
            X2 = x2;
            Y2 = y2;
            IsLine = isLine;
            Visible = true;
        }

        public float X1 { get; }

        public float Y1 { get; }

        public float ControlX1 { get; }

        public float ControlY1 { get; }

        public float ControlX2 { get; }

        public float ControlY2 { get; }

        public float X2 { get; }

        public float Y2 { get; }

        public bool IsLine { get; }

        public bool Visible { get; set; }

    }
}
