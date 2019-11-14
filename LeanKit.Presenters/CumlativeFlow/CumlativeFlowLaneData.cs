using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LeanKit.Presenters.CumlativeFlow
{
    public enum CumlativeFlowLaneType
    {
        Untyped,
        Ready,
        InProcess,
        Completed
    }

    public class CumlativeFlowLaneData
    {
        public CumlativeFlowLaneData(string title, List<int> points, CumlativeFlowLaneType type = CumlativeFlowLaneType.Untyped)
        {
            Title = title;
            PointsPerDay = points;
            Type = type;
        }

        public string Title { get; private set; }
        public List<int> PointsPerDay { get; private set; }
        public CumlativeFlowLaneType Type { get; private set; }
    }
}
