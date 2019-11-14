using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeanKitQuery
{

    class Program
    {
        static Dictionary<string, Type> GetPossibleQueries()
        {
            var queries = new Dictionary<string, Type>();

            queries.Add("list-board-ids", typeof(ListBoardIDsQuery));
            queries.Add("list-lane-ids", typeof(ListLaneIDsQuery));
            queries.Add("list-cards-in-lane", typeof(ListCardsInLaneQuery));
            queries.Add("day-lane-totals", typeof(DayLaneTotalsQuery));

            return queries;
        }

        static void ShowInvalidCommand(IEnumerable<string> commands, System.IO.TextWriter errorOutput)
        {
            errorOutput.WriteLine("Invalid command, options are:");
            errorOutput.WriteLine();
            foreach (var command in commands)
            {
                errorOutput.WriteLine(command);
            }
            errorOutput.WriteLine();
        }

        const int numberOfDefaultParameters = 1;

        static int Main(string[] args)
        {
            if (args.Length < numberOfDefaultParameters)
            {
                Console.Error.WriteLine("Need at least " + numberOfDefaultParameters + " arguments");
                return 1;
            }

            var queries = GetPossibleQueries();
            string[] parameters;
            var api = LeanKit.Model.LeanKitFactory.Instance.CreateApi("absw", Properties.Settings.Default.LeanKitUsername, Properties.Settings.Default.LeanKitPassword);

            parameters = new string[args.Length - numberOfDefaultParameters];
            Array.Copy(args, numberOfDefaultParameters, parameters, 0, args.Length - numberOfDefaultParameters);

            Type queryType;

            try
            {
                queryType = queries[args[0].ToLower()];
            }
            catch(KeyNotFoundException)
            {
                ShowInvalidCommand(queries.Keys, Console.Error);
                return 1;
            }

            try
            {
                ILeanKitQuery query = (ILeanKitQuery)Activator.CreateInstance(queryType);
                return query.RunQuery(api, parameters, Console.Out, Console.Error);
            }
            catch(LeanKit.API.Client.Library.Exceptions.UnauthorizedAccessException)
            {
                Console.Error.WriteLine("Invalid Username or Password");
                return 1;
            }
            catch(Exception ex)
            {
                Console.Error.WriteLine("Unknown exception thrown");
                Console.Error.WriteLine("Type        : " + ex.GetType().ToString());
                Console.Error.WriteLine("Message     : " + ex.Message);
                Console.Error.WriteLine("Stack trace : " + ex.StackTrace);
                return 1;

            }
        }
    }
}
