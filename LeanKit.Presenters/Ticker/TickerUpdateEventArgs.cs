using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LeanKit.Presenters.Ticker
{
    public class TickerUpdateEventArgs : EventArgs
    {
        public TickerUpdateEventArgs(string message)
        {
            Message = message;
        }

        public string Message { get; private set; }
    }
}
