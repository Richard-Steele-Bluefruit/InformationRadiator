using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HolidayCalendar.Presenters.Model;
using PresenterCommon;

namespace HolidayCalendar.Presenters.Tests
{
    internal class HolidayCalendarFactoryFake : HolidayCalendarFactory
    {
        public DateTime date = new DateTime(2016, 10, 28);
        public override DateTime Date()
        {
            return date.Date;
        }

        public LeaveDetails leaveDetails;
        public override LeaveDetails LoadLeaveDetailsFromDisk()
        {
            return leaveDetails;
        }

        public ITimer timer;
        public int timerInterval;
        public override ITimer CreateTimer(int secondsInterval)
        {
            timerInterval = secondsInterval;
            return timer;
        }

        public ITimetasticHolidayDownloader timetasticHolidayDownloader;
        public override ITimetasticHolidayDownloader CreateTimetasticHolidayDownloader()
        {
            return timetasticHolidayDownloader;
        }
    }
}
