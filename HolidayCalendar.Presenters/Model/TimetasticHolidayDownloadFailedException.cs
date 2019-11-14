using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HolidayCalendar.Presenters.Model
{
    public class TimetasticHolidayDownloadFailedException : Exception
    {
        public TimetasticHolidayDownloadFailedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
