using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PresenterCommon
{
    public interface ITimer
    {
        event EventHandler Tick;
    }
}
