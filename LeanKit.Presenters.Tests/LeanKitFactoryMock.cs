using System.Collections.Generic;
using LeanKit.Model;
using LeanKit.Model.LaneHistory;
using LeanKit.Model.Ticker;

namespace LeanKit.Presenters.Tests
{
    class LeanKitFactoryMock : LeanKitFactory
    {
        public ILeanKitPoints _points;
        public ILeanKitLanePointsHistory _laneHistory;

        public string _pointsHostName;
        public string _pointsUserName;
        public string _pointsPassword;

        public string _historyHostName;
        public string _historyUserName;
        public string _historyPassword;
        public long _historyBoardId;
        public List<long> _ignoredLanes;

        public string _tickerHostName;
        public string _tickerUserName;
        public string _tickerPassword;
        public long _tickerBoardId;
        public long _tickerLaneId;

        public PresenterCommon.ITimer _timer;
        public ILeanKitTicker _ticker;

        public double _interval;

        public override ILeanKitPoints CreateLeanKitPoints(string hostName, string userName, string password)
        {
            var points = _points;
            _points = null;
            _pointsHostName = hostName;
            _pointsUserName = userName;
            _pointsPassword = password;
            return points;
        }

        public override PresenterCommon.ITimer CreateTimer(double interval)
        {
            var timer = _timer;
            _timer = null;
            _interval = interval;
            return timer;
        }

        public override Model.LaneHistory.ILeanKitLanePointsHistory CreateLanePointsHistory(string hostName, string userName, string password, long boardId, List<long> ignoredLanes)
        {
            var laneHistory = _laneHistory;
            _laneHistory = null;
            _historyHostName = hostName;
            _historyUserName = userName;
            _historyPassword = password;
            _historyBoardId = boardId;
            _ignoredLanes = ignoredLanes;
            return laneHistory;
        }

        public override Model.Ticker.ILeanKitTicker CreateTicker(string hostName, string userName, string password, long boardId, long laneId)
        {
            var ticker = _ticker;
            _ticker = null;
            _tickerHostName = hostName;
            _tickerUserName = userName;
            _tickerPassword = password;
            _tickerBoardId = boardId;
            _tickerLaneId = laneId;
            return ticker;
        }
    }
}
