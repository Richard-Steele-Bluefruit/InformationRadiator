using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace HolidayCalendar.Presenters
{
    public class LeaveDetails
    {
        public struct LeaveDay
        {
            [XmlAttribute]
            public DateTime Date;

            [XmlAttribute]
            public string Name;
        }

        public DateTime DownloadDateTime { get; set; }

        [XmlArrayItem("LeaveDay")]
        public List<LeaveDay> LeaveDays { get; set; }

        public LeaveDetails()
        {
            LeaveDays = new List<LeaveDay>();
            DownloadDateTime = DateTime.Now;
        }

        public void Save(string path)
        {
            var serializer = new XmlSerializer(GetType());
            using (var writer = new System.IO.FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                serializer.Serialize(writer, this);
            }
        }

        public static LeaveDetails Load(string path)
        {
            var serializer = new XmlSerializer(typeof(LeaveDetails));
            using (var reader = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return (LeaveDetails) serializer.Deserialize(reader);
            }
        }
    }
}
