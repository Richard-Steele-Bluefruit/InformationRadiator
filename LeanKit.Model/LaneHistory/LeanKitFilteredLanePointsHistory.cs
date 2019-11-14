using System;
using System.Collections.Generic;
using System.Linq;

using LeanKit.API.Client.Library.TransferObjects;

namespace LeanKit.Model.LaneHistory
{
    public class LeanKitFilteredLanePointsHistory : ILeanKitLanePointsHistory
    {
        private ILeanKitLanePointsHistory _lanePointsHistory;
        private List<long> _ignoredLanes;

        System.Collections.ObjectModel.ReadOnlyCollection<LanePointsHistory> _laneHistory;
        
        bool _updated;

        public LeanKitFilteredLanePointsHistory(ILeanKitLanePointsHistory lanePointsHistory, List<long> ignoredLanes)
        {
            _lanePointsHistory = lanePointsHistory;
            _ignoredLanes = ignoredLanes;
        }

        private bool IsLaneIncluded(long id, List<Lane> allLanes)
        {
            if (_ignoredLanes.Exists(p => p == id))
            {
                return false;
            }

            var lane = allLanes.FirstOrDefault(p => p.Id == id);
            if(lane != null && lane.ParentLaneId != 0)
            {
                return IsLaneIncluded(lane.ParentLaneId, allLanes);
            }
            return true;
        }

        public System.Collections.ObjectModel.ReadOnlyCollection<LanePointsHistory> LaneHistory
        {
            get
            {
                if(_updated)
                {
                    var allLanes = new List<Lane>(_lanePointsHistory.Board.Backlog);
                    allLanes.AddRange(_lanePointsHistory.Board.Lanes);
                    allLanes.AddRange(_lanePointsHistory.Board.Archive);

                    var laneHistory = new List<LanePointsHistory>();

                    foreach(var lane in _lanePointsHistory.LaneHistory)
                    {
                        if(IsLaneIncluded(lane.Id, allLanes))
                        {
                            laneHistory.Add(lane);
                        }
                    }

                    _laneHistory = laneHistory.AsReadOnly();

                    _updated = false;
                }
                return _laneHistory;
            }
        }

        public API.Client.Library.TransferObjects.Board Board
        {
            get { return _lanePointsHistory.Board; }
        }

        public void Update(int numberOfDaysHistory = 10)
        {
            _lanePointsHistory.Update(numberOfDaysHistory);
            _updated = true;
        }
    }
}
