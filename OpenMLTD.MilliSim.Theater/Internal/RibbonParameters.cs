namespace OpenMLTD.MilliSim.Theater.Internal {
    internal struct RibbonParameters {

        internal RibbonParameters(float x1, float y1, float x2, float y2) {
            X1 = x1;
            Y1 = y1;
            ControlX1 = 0;
            ControlY1 = 0;
            ControlX2 = 0;
            ControlY2 = 0;
            X2 = x2;
            Y2 = y2;
            IsLine = true;
        }

        internal RibbonParameters(float x1, float y1, float cx1, float cy1, float cx2, float cy2, float x2, float y2) {
            X1 = x1;
            Y1 = y1;
            ControlX1 = cx1;
            ControlY1 = cy1;
            ControlX2 = cx2;
            ControlY2 = cy2;
            X2 = x2;
            Y2 = y2;
            IsLine = false;
        }

        internal RibbonParameters(float x1, float y1, float cx1, float cy1, float cx2, float cy2, float x2, float y2, bool isLine) {
            X1 = x1;
            Y1 = y1;
            ControlX1 = cx1;
            ControlY1 = cy1;
            ControlX2 = cx2;
            ControlY2 = cy2;
            X2 = x2;
            Y2 = y2;
            IsLine = isLine;
        }

        internal float X1 { get; set; }

        internal float Y1 { get; set; }

        internal float ControlX1 { get; set; }

        internal float ControlY1 { get; set; }

        internal float ControlX2 { get; set; }

        internal float ControlY2 { get; set; }

        internal float X2 { get; set; }

        internal float Y2 { get; set; }

        internal bool IsLine { get; set; }

    }
}
