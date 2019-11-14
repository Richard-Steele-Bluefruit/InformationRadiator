using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LeanKit.Model.Tests
{
    public static class LeanKitConversions
    {

        public static string DateTimeToCardEventDateTime(DateTime t)
        {
            //"28/10/2014 at 03:41:13 PM"
            return t.ToShortDateString() + " at " + t.ToLongTimeString();
        }

    }
}
