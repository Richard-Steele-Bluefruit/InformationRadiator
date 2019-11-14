using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PresenterCommon
{
    public interface IDayUpdateMonitor
    {
        event EventHandler DayChanged;
    }
}
