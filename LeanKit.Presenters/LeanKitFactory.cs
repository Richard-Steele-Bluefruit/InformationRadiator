using System.Collections.Generic;
using PresenterCommon;
using LeanKit.Model;
using LeanKit.Model.LaneHistory;
using LeanKit.Model.Ticker;

namespace LeanKit.Presenters
{
    public class LeanKitFactory
    {
        private static LeanKitFactory _instance;

        public static LeanKitFactory Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new LeanKitFactory();
                return _instance;
            }
            internal set
            {
                _instance = value;
            }
        }

        public virtual ILeanKitPoints CreateLeanKitPoints(string hostName, string userName, string password)
        {
            return new LeanKitPoints(hostName, userName, password);
        }

        public virtual ITimer CreateTimer(double interval)
        {
            return new DotNetTimer(interval);
        }

        public virtual ILeanKitLanePointsHistory CreateLanePointsHistory(string hostName, string userName, string password, long boardId, List<long> ignoredLanes)
        {
            var history = new LeanKitLanePointsHistory(hostName, userName, password, boardId);
            return new LeanKitFilteredLanePointsHistory(history, ignoredLanes);
        }

        public virtual ILeanKitTicker CreateTicker(string hostName, string userName, string password, long boardId, long laneId)
        {
            return new LeanKitTicker(hostName, userName, password, boardId, laneId);
        }
    }
}
