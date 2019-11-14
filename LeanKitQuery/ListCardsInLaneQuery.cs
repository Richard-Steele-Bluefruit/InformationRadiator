using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using LeanKit.API.Client.Library;
using LeanKit.API.Client.Library.TransferObjects;

namespace LeanKitQuery
{
    public class ListCardsInLaneQuery : ILeanKitQuery
    {
        public int RunQuery(ILeanKitApi client, string[] parameters, TextWriter output, TextWriter errorOutput)
        {
            if (parameters.Length < 2)
            {
                errorOutput.WriteLine("No board ID and/or lane ID specified");
                return 1;
            }
            var cards = client.GetBoard(long.Parse(parameters[0])).GetLaneById(long.Parse(parameters[1])).Cards;

            foreach (var card in cards)
            {
                output.WriteLine(card.Title + "\t" + card.Description);
            }

            return 0;
        }

    }
}
