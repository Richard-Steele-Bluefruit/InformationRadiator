using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PresenterCommon.Plugin;

namespace HolidayCalendar.Views
{
    public class TimetasticHolidayCalendarPlugin : IInformationRadiatorItemPlugin
    {
        public string ItemType
        {
            get { return "TimetasticHolidayCalendar"; }
        }

        public Type PresenterType
        {
            get { return typeof (HolidayCalendar.Presenters.TimetasticHolidayCalendarPresenter); }
        }

        public Type ViewType
        {
            get { return typeof(HolidayCalendarView); }
        }
    }
}
