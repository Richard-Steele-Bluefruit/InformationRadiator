using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using PresenterCommon;
using PresenterCommon.Configuration;

namespace HolidayCalendar.Presenters
{
    public class HolidayCalendarPresenter : IHolidayCalendarPresenter
    {
        private readonly ITimer _timer;
        private const int _updateInterval = 5 * 60 * 1000;

        public HolidayCalendarPresenter(InformationRadiatorItemConfiguration configuration)
        {
            _timer = HolidayCalendarFactory.Instance.CreateTimer(_updateInterval);
            _timer.Tick += timer_Tick;
        }

        void timer_Tick(object sender, EventArgs e)
        {
            UpdateHolidayCalendar();
        }

        public event EventHandler ErrorLoadingHolidayInformation;

        private void OnErrorLoadingHolidayInformation()
        {
            var handler = ErrorLoadingHolidayInformation;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        public event EventHandler<HolidayCalendarUpdateEventArgs> HolidayCalendarUpdate;

        private void OnHolidayCalendarUpdate(DateTime updateTime, List<HolidayCalendarDay> days)
        {
            var handler = HolidayCalendarUpdate;
            if (handler != null)
            {
                handler(this, new HolidayCalendarUpdateEventArgs(updateTime, days));
            }
        }

        public void UpdateHolidayCalendar()
        {
            try
            {
                var days = new List<HolidayCalendarDay>();
                var leave = HolidayCalendarFactory.Instance.LoadLeaveDetailsFromDisk();

                foreach (var leaveDay in leave.LeaveDays)
                {
                    if (leaveDay.Date.Date < HolidayCalendarFactory.Instance.Date())
                        continue;
                    if (leaveDay.Date.Date > HolidayCalendarFactory.Instance.Date().AddDays(13))
                        continue;

                    HolidayCalendarDay calendarDay = days.FirstOrDefault(d => d.Date.Date == leaveDay.Date.Date);
                    if (calendarDay == null)
                    {
                        calendarDay = new HolidayCalendarDay(leaveDay.Date.Date);
                        days.Add(calendarDay);
                    }
                    calendarDay.PeopleOnLeave.Add(leaveDay.Name);
                }

                OnHolidayCalendarUpdate(leave.DownloadDateTime, days);
            }
            catch
            {
                OnErrorLoadingHolidayInformation();
            }
        }
    }
}
