using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HolidayCalendar.Presenters.Model;
using PresenterCommon;
using PresenterCommon.Configuration;

namespace HolidayCalendar.Presenters
{
    public class TimetasticHolidayCalendarPresenter : IHolidayCalendarPresenter
    {
        private readonly ITimer _timer;
        private const int _updateInterval = 5 * 60 * 1000;
        private readonly Model.ITimetasticHolidayDownloader timetasticDownloader;

        public TimetasticHolidayCalendarPresenter(InformationRadiatorItemConfiguration configuration)
        {
            timetasticDownloader = HolidayCalendarFactory.Instance.CreateTimetasticHolidayDownloader();
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

        private void AddUsersHolidayToHolidayList(TimetasticUser user,
            List<TimetasticHolidayEntry> userHoliday,
            List<HolidayCalendarDay> days)
        {
            Func<DateTime, DateTime, DateTime, bool> inRange =
                (date, minDate, maxDate) => (date >= minDate) && (date <= maxDate);

            foreach (var entry in userHoliday)
            {
                foreach (var day in days)
                {
                    if (inRange(day.Date, entry.startDate, entry.endDate))
                    {
                        day.PeopleOnLeave.Add(user.firstname + " " + user.surname);
                    }
                }
            }
        }

        private void Update(object notUsed)
        {
            try
            {
                var days = new List<HolidayCalendarDay>();
                var users = timetasticDownloader.DownloadUsers();
                var now = HolidayCalendarFactory.Instance.Date();

                for (DateTime date = now; date < now.AddDays(14); date = date.AddDays(1))
                {
                    days.Add(new HolidayCalendarDay(date));
                }

                foreach (var user in users)
                {
                    var userHoliday = timetasticDownloader.DownloadAUsersHoliday(user.id, now);
                    AddUsersHolidayToHolidayList(user, userHoliday, days);
                }

                OnHolidayCalendarUpdate(DateTime.Now, days);
            }
            catch (Model.TimetasticHolidayDownloadFailedException)
            {
                OnErrorLoadingHolidayInformation();
            }
        }

        public void UpdateHolidayCalendar()
        {
            System.Threading.ThreadPool.QueueUserWorkItem(Update);
        }
    }
}
