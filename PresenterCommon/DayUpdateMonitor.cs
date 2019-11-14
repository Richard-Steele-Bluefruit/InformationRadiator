using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PresenterCommon
{
    public class DayUpdateMonitor : IDayUpdateMonitor
    {
        private System.Timers.Timer _timer;
        private DateTime _previousDate;

        public event EventHandler DayChanged;

        protected void OnDayChanged()
        {
            EventHandler ev = DayChanged;

            if (ev != null)
                ev(this, EventArgs.Empty);
        }

        public DayUpdateMonitor()
        {
            _previousDate = DateTime.Now.Date;
            _timer = new System.Timers.Timer();
            _timer.Interval = 60000;
            _timer.Elapsed += _timer_Elapsed;
            _timer.Start();
        }

        void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (DateTime.Now.Date != _previousDate)
            {
                _previousDate = DateTime.Now.Date;
                OnDayChanged();
            }
        }
    }
}
