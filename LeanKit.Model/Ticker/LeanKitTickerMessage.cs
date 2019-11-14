using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LeanKit.Model.Ticker
{
    public class LeanKitTickerMessage
    {
        public string Message { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? DueDate { get; set; }
    }
}
