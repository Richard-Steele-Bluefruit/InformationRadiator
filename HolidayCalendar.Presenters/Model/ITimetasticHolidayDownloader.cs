using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HolidayCalendar.Presenters.Model
{
    public interface ITimetasticHolidayDownloader
    {
        List<TimetasticUser> DownloadUsers();
        List<TimetasticHolidayEntry> DownloadAUsersHoliday(string id, DateTime fromDate);
    }
}
