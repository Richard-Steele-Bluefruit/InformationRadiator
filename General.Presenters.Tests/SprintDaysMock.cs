using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Presenters.Tests
{
    class SprintDaysMock : General.Model.ISprintDays
    {
        public int _sprintDay = 0;
        public bool _sprintDayRead = false;
        public DateTime _startDate = DateTime.MinValue;
        public DateTime _currentDate = DateTime.MinValue;
        public int _daysInSprintReadCount = 0;
        public int _daysInSprint = 10;

        public DateTime StartDate
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                _startDate = value;
            }
        }

        public DateTime CurrentDate
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                _currentDate = value;
            }
        }

        public int SprintDay
        {
            get
            {
                _sprintDayRead = true;
                return _sprintDay;
            }
        }

        public int DaysInSprint
        {
            get
            {
                _daysInSprintReadCount++;
                return _daysInSprint;
            }
            set
            {
                _daysInSprint = value;
            }
        }
    }
}
