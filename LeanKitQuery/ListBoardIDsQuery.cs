using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using LeanKit.API.Client.Library;
using LeanKit.API.Client.Library.TransferObjects;

namespace LeanKitQuery
{
    public class ListBoardIDsQuery : ILeanKitQuery
    {
        public int RunQuery(ILeanKitApi client, string[] parameters, TextWriter output, TextWriter errorOutput)
        {
            var boards = client.GetBoards();

            foreach (var board in boards)
            {
                if(parameters.Length > 0)
                {
                    bool found = true;
                    foreach(var parameter in parameters)
                    {
                        if(!board.Title.ToLower().Contains(parameter.ToLower()))
                        {
                            found = false;
                            break;
                        }
                    }
                    if (!found)
                        continue;
                }
                output.WriteLine(board.Id + " - " + board.Title);
            }

            return 0;
        }

    }
}
