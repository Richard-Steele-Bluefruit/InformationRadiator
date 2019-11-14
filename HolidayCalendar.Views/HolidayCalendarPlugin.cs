using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PresenterCommon.Plugin;

namespace HolidayCalendar.Views
{
    public class HolidayCalendarPlugin : IInformationRadiatorItemPlugin
    {
        public string ItemType
        {
            get { return "HolidayCalendar"; }
        }

        public Type PresenterType
        {
            get { return typeof (HolidayCalendar.Presenters.HolidayCalendarPresenter); }
        }

        public Type ViewType
        {
            get { return typeof(HolidayCalendarView); }
        }
    }
}
