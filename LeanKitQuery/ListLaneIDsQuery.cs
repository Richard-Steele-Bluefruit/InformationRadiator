using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using LeanKit.API.Client.Library;
using LeanKit.API.Client.Library.TransferObjects;

namespace LeanKitQuery
{
    public class ListLaneIDsQuery : ILeanKitQuery
    {
        public void OutputLanes(IEnumerable<Lane> lanes, TextWriter output, string indent, long id)
        {
            var unorderedLanes = from lane in lanes
                    where lane.ParentLaneId == id
                    select lane;

            var orderedLanes = unorderedLanes.OrderBy(l => l.Index);

            foreach (var lane in orderedLanes)
            {
                output.WriteLine(indent + lane.Title + " - " + lane.Id.ToString());
                OutputLanes(lanes, output, indent + "\t", lane.Id ?? 0);
            }
        }

        public int RunQuery(ILeanKitApi client, string[] parameters, TextWriter output, TextWriter errorOutput)
        {
            if(parameters.Length < 1)
            {
                errorOutput.WriteLine("No board ID specified");
                return 1;
            }

            var backlog = client.GetBoard(long.Parse(parameters[0])).Backlog.ToArray();
            var lanes = client.GetBoard(long.Parse(parameters[0])).Lanes.ToArray();
            var archive = client.GetBoard(long.Parse(parameters[0])).Archive.ToArray();
            OutputLanes(backlog, output, "", 0);
            OutputLanes(lanes, output, "", 0);
            OutputLanes(archive, output, "", 0);

            return 0;
        }
    }
}
