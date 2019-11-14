using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace HolidayCalendar.Presenters.Model
{
    class TimetasticHolidayDownloader : ITimetasticHolidayDownloader
    {
        private class TimetasticHolidayDetails
        {
            public List<TimetasticHolidayEntry> holidays { get; set; }
        }

        private System.Net.WebClient client;

        public TimetasticHolidayDownloader()
        {
            client = new System.Net.WebClient();
            client.Headers.Add("Authorization", "Bearer f582ef5e-9aca-4648-b9dc-39131b36d7ca");
        }

        public List<TimetasticUser> DownloadUsers()
        {
            try
            {
                var data = client.DownloadString("https://app.timetastic.co.uk/api/users");

                return Newtonsoft.Json.JsonConvert.DeserializeObject<List<TimetasticUser>>(data);
            }
            catch (Exception ex)
            {
                throw new TimetasticHolidayDownloadFailedException("Error to download users", ex);
            }
        }

        public List<TimetasticHolidayEntry> DownloadAUsersHoliday(string id, DateTime fromDate)
        {
            try
            {
                var query = HttpUtility.ParseQueryString(string.Empty);
                query["userids"] = id;
                query["start"] = fromDate.ToString("yyyy-MM-dd");
                var data = client.DownloadString("https://app.timetastic.co.uk/api/holidays?" + query);

                var holiday = Newtonsoft.Json.JsonConvert.DeserializeObject<TimetasticHolidayDetails>(data);

                return holiday.holidays;
            }
            catch (Exception ex)
            {
                throw new TimetasticHolidayDownloadFailedException("Error to download holiday entries", ex);
            }
        }
    }
}
