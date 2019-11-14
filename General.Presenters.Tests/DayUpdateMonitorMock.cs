using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Presenters.Tests
{
    class DayUpdateMonitorMock : PresenterCommon.IDayUpdateMonitor
    {
        public event EventHandler DayChanged;

        public void OnDayChanged()
        {
            DayChanged(this, EventArgs.Empty);
        }
    }
}
