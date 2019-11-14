using System;
using System.Collections.Generic;
using System.Drawing;

namespace LeanKit.Presenters.CumlativeFlow
{
    public class CumaltiveFlowDataUpdateEventArgs : EventArgs
    {
        public CumaltiveFlowDataUpdateEventArgs(Color readyStartColour, Color readyEndColour,
            Color inProcessStartColour, Color inProcessEndColour,
            Color completeStartColour, Color completeEndColour)
        {
            Lanes = new List<CumlativeFlowLaneData>();

            ReadyStartColour = readyStartColour;
            ReadyEndColour = readyEndColour;
            InProcessStartColour = inProcessStartColour;
            InProcessEndColour = inProcessEndColour;
            CompleteStartColour = completeStartColour;
            CompleteEndColour = completeEndColour;
        }

        public List<CumlativeFlowLaneData> Lanes { get; private set; }

        public Color ReadyStartColour { get; private set; }
        public Color ReadyEndColour { get; private set; }
        public Color InProcessStartColour { get; private set; }
        public Color InProcessEndColour { get; private set; }
        public Color CompleteStartColour { get; private set; }
        public Color CompleteEndColour { get; private set; }
    }
}
