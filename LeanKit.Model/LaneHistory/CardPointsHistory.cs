using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LeanKit.Model.LaneHistory
{
    public class CardPointsHistory
    {
        public CardPointsHistory(long landId, int points)
        {
            LaneId = landId;
            Points = points;
        }

        public long LaneId { get; private set; }
        public int Points { get; private set; }
    }
}
