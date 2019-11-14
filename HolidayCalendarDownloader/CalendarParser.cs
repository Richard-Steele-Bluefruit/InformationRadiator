using System;
using System.Linq;
using HolidayCalendar.Presenters;
using HtmlAgilityPack;

namespace HolidayCalendarDownloader
{
    public class CalendarParser
    {
        public static LeaveDetails ParseEWAHtmlFile(string path)
        {
            HtmlDocument document = new HtmlDocument();
            document.DetectEncodingAndLoad(path);

            return ParseHtmlDocument(document);
        }

        public static LeaveDetails ParseEWAHtmlString(string html)
        {
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(html);

            return ParseHtmlDocument(document);
        }

        public static LeaveDetails ParseHtmlDocument(HtmlDocument document)
        {
            var details = new LeaveDetails();

            var calendarTable =
                document.DocumentNode.Descendants()
                    .First(n => n.Attributes["class"] != null && n.Attributes["class"].Value.Contains("Calendar"));

            var monthTables =
                calendarTable.Descendants()
                    .Where(n => n.Attributes["class"] != null && n.Attributes["class"].Value.Contains("Grid"));

            DateTime d = new DateTime(DateTime.Now.Year, 1, 1);

            foreach (var monthTable in monthTables)
            {
                var days =
                    monthTable.Descendants()
                        .Where(
                            n =>
                                n.Attributes["class"] != null &&
                                (n.Attributes["class"].Value == "CalendarDay" ||
                                 n.Attributes["class"].Value == "CalendarWeekend"));

                foreach (var day in days)
                {
                    var events = day.Descendants()
                        .Where(
                            n => n.Attributes["class"] != null && n.Attributes["class"].Value.Contains("CalendarEvent"));


                    foreach (var calendarEvent in events)
                    {
                        var name = calendarEvent.ChildNodes["a"].Attributes["title"].Value;
                        name = name.Substring(0, name.IndexOf(" /"));
                        details.LeaveDays.Add(new LeaveDetails.LeaveDay() {Date = d, Name = name});
                    }

                    d = d.AddDays(1);
                }
            }

            return details;
        }
    }
}
