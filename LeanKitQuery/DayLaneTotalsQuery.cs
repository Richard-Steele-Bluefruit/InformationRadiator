using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using LeanKit.API.Client.Library;
using LeanKit.API.Client.Library.TransferObjects;
using LeanKit.API.Client.Library.Enumerations;

namespace LeanKitQuery
{
    public class DayLaneTotalsQuery : ILeanKitQuery
    {
        private long _boardId;
        private LeanKit.Model.LaneHistory.LeanKitLanePointsHistory _lanePoints;
        private TextWriter _output;

        private void OutputLanes(IList<Lane> allLanes, long id)
        {
            var lanes = (from lane in allLanes
                        where lane.ParentLaneId == id
                        select new { Id = lane.Id, Title = lane.Title, ChildLaneIds = lane.ChildLaneIds, Index = lane.Index })
                        .OrderBy((m) => { return m.Index; });

            foreach(var lane in lanes)
            {
                var laneHistory = _lanePoints.LaneHistory.First(m => m.Id == lane.Id);
                _output.Write((lane.Id ?? 0).ToString("0000000000") + ", ");
                foreach(var points in laneHistory.PointsPerDay)
                {
                    _output.Write(points.ToString("00") + ", ");
                }
                _output.WriteLine(lane.Title);

                OutputLanes(allLanes, lane.Id ?? 0);
            }
        }

        public int RunQuery(ILeanKitApi client, string[] parameters, TextWriter output, TextWriter errorOutput)
        {
            if (parameters.Length < 1)
            {
                errorOutput.WriteLine("No board ID specified");
                return 1;
            }
            if(!long.TryParse(parameters[0], out _boardId))
            {
                errorOutput.WriteLine("Invalid board ID specified");
                return 1;
            }

            int numberOfDaysHistory = 10;
            if (parameters.Length >= 2)
            {
                if(!int.TryParse(parameters[1], out numberOfDaysHistory))
                {
                    errorOutput.WriteLine("Invalid number of days specified");
                    return 1;
                }
            }

            _output = output;
            _lanePoints = new LeanKit.Model.LaneHistory.LeanKitLanePointsHistory(client, _boardId);
            _lanePoints.Update(numberOfDaysHistory);

            OutputLanes(_lanePoints.Board.Backlog, 0);
            OutputLanes(_lanePoints.Board.Lanes, 0);
            OutputLanes(_lanePoints.Board.Archive, 0);

            return 0;
        }
    }
}
