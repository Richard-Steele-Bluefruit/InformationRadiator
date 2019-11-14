using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeanKit.API.Client.Library;

namespace LeanKitQuery
{
    public interface ILeanKitQuery
    {
        int RunQuery(ILeanKitApi client, string[] parameters, TextWriter output, TextWriter errorOutput);
    }
}
