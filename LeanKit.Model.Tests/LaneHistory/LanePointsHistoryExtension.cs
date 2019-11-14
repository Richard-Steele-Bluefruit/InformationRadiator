using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using LeanKit.Model.LaneHistory;

namespace LeanKit.Model.Tests.LaneHistory
{
    public static class LanePointsHistoryExtension
    {
        public static void AssertLaneHistoryIs(this LanePointsHistory lane, List<int> expectedPoints, string laneName, TypeOfLane? laneType = null)
        {
            string description;

            if (laneName != null)
            {
                Assert.AreEqual(expectedPoints.Count, lane.PointsPerDay.Count, "Lane \"" + laneName + "\" has an invalid PointsPerDay count");
                description = "Invalid number of days in lane \"" + laneName + "\" on day ";
            }
            else
            {
                Assert.AreEqual(expectedPoints.Count, lane.PointsPerDay.Count, "Invalid PointsPerDay count");
                description = "Invalid number of days on day ";
            }

            if(laneType!= null)
            {
                Assert.AreEqual(laneType.Value, lane.Type);
            }

            for (int i = 0; i < expectedPoints.Count; i++)
            {
                Assert.AreEqual(expectedPoints[i], lane.PointsPerDay[i], description + i);
            }

            if(laneName != null)
            {
                Assert.AreEqual(laneName, lane.Title);
            }
        }
    }
}
