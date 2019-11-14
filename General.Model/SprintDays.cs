using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace General.Model
{
    public class SprintDays : ISprintDays
    {
        public SprintDays()
        {
            DaysInSprint = 10;
        }

        public int DaysInSprint { get { return 10; } set { } }

        public DateTime StartDate { get; set; }

        [XmlIgnore()]
        public DateTime CurrentDate { get; set; }

        private bool IsWeekDay(DayOfWeek day)
        {
            switch (day)
            {
                case DayOfWeek.Saturday:
                case DayOfWeek.Sunday:
                    return false;
                default:
                    return true;
            }
        }

        [XmlIgnore()]
        public int SprintDay
        {
            get
            {
                TimeSpan difference = CurrentDate - StartDate;
                int TotalDays = ((DaysInSprint / 5) * 2) + DaysInSprint;
                DateTime sprintStartDate = StartDate.AddDays((difference.Days / TotalDays) * TotalDays);
                int result = 0;
                int day;

                for (day = 0; day < (difference.Days % TotalDays) + 1; day++)
                {
                    if (IsWeekDay(sprintStartDate.AddDays(day).DayOfWeek))
                        result++;
                }

                return result;
            }
        }
    }
}
