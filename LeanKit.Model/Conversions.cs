using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LeanKit.API.Client.Library.TransferObjects;
using LeanKit.API.Client.Library.Enumerations;

namespace LeanKit.Model
{
    static class Conversions
    {
        private static DateTime StringDateTimeToDateTime(string t)
        {
            DateTime date = DateTime.Parse(t.Substring(0, 10) + " " + t.Substring(15));
            return date;
        }

        public static DateTime ConvertedDateTime(this CardEvent t)
        {
            return StringDateTimeToDateTime(t.DateTime);
        }

        public static DateTime? ConvertedStartDate(this CardView c)
        {
            if (string.IsNullOrEmpty(c.StartDate))
            {
                return null;
            }
            return DateTime.Parse(c.StartDate);
        }

        public static DateTime? ConvertedDueDate(this CardView c)
        {
            if(string.IsNullOrEmpty(c.DueDate))
            {
                return null;
            }
            return DateTime.Parse(c.DueDate);
        }

        public static DateTime? ConvertedLastActivity(this CardView c)
        {
            if (string.IsNullOrEmpty(c.LastActivity))
            {
                return null;
            }
            return DateTime.Parse(c.LastActivity);
        }

        public static EventType CardEventType(this CardEvent card)
        {
            EventType ev;
            if (!Enum.TryParse<EventType>(card.Type.Substring(0, card.Type.Length - 5), true, out ev))
            {
                ev = EventType.Unrecognized;
            }
            return ev;
        }

    }
}
