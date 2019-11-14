using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HolidayCalendar.Presenters
{
    public class HolidayCalendarDay
    {
        public HolidayCalendarDay(DateTime date)
        {
            Date = date;
            PeopleOnLeave = new List<string>();
        }

        public DateTime Date { get; private set; }
        public List<string> PeopleOnLeave { get; private set; }
    }

    public class HolidayCalendarUpdateEventArgs : EventArgs
    {
        public DateTime DownloadedTime { get; private set; }
        public List<HolidayCalendarDay> Days { get; private set; }

        public HolidayCalendarUpdateEventArgs(DateTime downloadedTime, List<HolidayCalendarDay> days)
        {
            DownloadedTime = downloadedTime;
            Days = days;
        }
    }

    public interface IHolidayCalendarPresenter
    {
        event EventHandler ErrorLoadingHolidayInformation;
        event EventHandler<HolidayCalendarUpdateEventArgs> HolidayCalendarUpdate;
        void UpdateHolidayCalendar();

    }
}
