using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LeanKit.API.Client.Library;
using LeanKit.API.Client.Library.TransferObjects;

namespace LeanKit.Model
{
    public class LeanKitPoints : ILeanKitPoints
    {
        private ILeanKitApi _api;

        public LeanKitPoints(string hostName, string userName, string password)
        {
            _api = LeanKitFactory.Instance.CreateApi(hostName, userName, password);
        }

        private int CalculateCardsSizeForLaneType(IList<Lane> allLanes, LaneType type)
        {
            var lanes = from lane in allLanes
                        where lane.Type == type
                        select lane;

            return lanes.Sum(l => l.Cards.Sum(c => Math.Max(c.Size, 1)));
        }

        private void AddLaneUnlessFiltered(IList<Lane> lanes, IList<long> ignoresLanes, Lane lane, IList<Lane> result)
        {
            if (ignoresLanes.Contains(lane.Id ?? -1))
                return;

            result.Add(lane);

            var childLanes = from child in lanes
                             where child.ParentLaneId == lane.Id
                             select child;

            foreach (var child in childLanes)
                AddLaneUnlessFiltered(lanes, ignoresLanes, child, result);
        }

        private IList<Lane> GetFilteredLanes(Board board, IList<long> ignoresLanes)
        {
            var parentLanes = from lane in board.Lanes
                              where lane.ParentLaneId == 0
                              select lane;

            var result = new List<Lane>();

            foreach (Lane lane in parentLanes)
                AddLaneUnlessFiltered(board.Lanes, ignoresLanes, lane, result);

            return result;
        }

        public void Update(long boardId, IList<long> ignoresLanes)
        {
            var board = _api.GetBoard(boardId);

            var lanes = GetFilteredLanes(board, ignoresLanes);

            ReadyPoints = CalculateCardsSizeForLaneType(lanes, LaneType.Ready);
            InProgressPoints = CalculateCardsSizeForLaneType(lanes, LaneType.InProcess);
            CompletePoints = CalculateCardsSizeForLaneType(lanes, LaneType.Completed);
            UntypedPoints = CalculateCardsSizeForLaneType(lanes, LaneType.Untyped);
        }

        public int ReadyPoints { get; private set; }
        public int InProgressPoints { get; private set; }
        public int CompletePoints { get; private set; }
        public int UntypedPoints { get; private set; }
    }
}
