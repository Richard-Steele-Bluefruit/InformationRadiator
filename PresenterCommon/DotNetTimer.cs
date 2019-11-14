using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PresenterCommon
{
    public class DotNetTimer : ITimer
    {
        private System.Timers.Timer _timer;

        public event EventHandler Tick;

        protected void OnTick()
        {
            var ev = Tick;
            if (ev != null)
                ev(this, EventArgs.Empty);
        }

        public DotNetTimer(double interval)
        {
            _timer = new System.Timers.Timer();
            _timer.Elapsed += _timer_Elapsed;
            _timer.Interval = interval;
            _timer.Start();
        }

        void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            OnTick();
        }

    }
}
