using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LeanKit.Model.Ticker
{
    public interface ILeanKitTicker
    {
        IList<LeanKitTickerMessage> GetMessages();
    }
}
