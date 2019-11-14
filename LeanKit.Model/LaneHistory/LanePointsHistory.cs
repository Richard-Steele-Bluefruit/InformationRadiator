using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace LeanKit.Model.LaneHistory
{
    public enum TypeOfLane
    {
        Untyped,
        Ready,
        InProcess,
        Completed
    }

    [DebuggerDisplay("{GetType().Name,nq} - Id: {Id}, PointsPerDay.Count: {PointsPerDay.Count}")]
    public class LanePointsHistory
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public IList<int> PointsPerDay { get; set; }
        public TypeOfLane Type { get; set; }
    }
}
