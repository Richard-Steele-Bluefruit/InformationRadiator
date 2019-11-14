using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HolidayCalendar.Presenters.Model;
using PresenterCommon;

namespace HolidayCalendar.Presenters
{
    public class HolidayCalendarFactory
    {
        private static HolidayCalendarFactory _instance;

        public static HolidayCalendarFactory Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new HolidayCalendarFactory();
                return _instance;
            }
            internal set
            {
                _instance = value;
            }

        }

        public virtual DateTime Date()
        {
            return DateTime.Now.Date;
        }

        public virtual LeaveDetails LoadLeaveDetailsFromDisk()
        {
            return LeaveDetails.Load(Path.GetDirectoryName(Assembly.GetAssembly(GetType()).Location) + Path.DirectorySeparatorChar + "Leave.xml");
        }

        public virtual ITimer CreateTimer(int secondsInterval)
        {
            return new DotNetTimer((double)secondsInterval);
        }

        public virtual ITimetasticHolidayDownloader CreateTimetasticHolidayDownloader()
        {
            return new Model.TimetasticHolidayDownloader();
        }
    }
}
