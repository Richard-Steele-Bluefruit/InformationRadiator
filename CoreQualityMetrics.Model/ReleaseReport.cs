using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreQualityMetrics.Model
{
    public class ReleaseReport
    {
        private class ReleaseReportJSON
        {
            public string project_id { get; set; }
            public string version { get; set; }
            public string date { get; set; }
            public Dictionary<string, string> metrics { get; set; }
        }

        public ReleaseReport(string jsonData, Dictionary<string, ProjectsAndMetrics.Metric> metrics)
        {
            var textReader = new System.IO.StringReader(jsonData);
            var reader = new Newtonsoft.Json.JsonTextReader(textReader);
            var decode = new Newtonsoft.Json.JsonSerializer();

            Decoded = decode.Deserialize<ReleaseReportJSON>(reader);

            BooleanMetricResults = new Dictionary<string, bool>();
            foreach (var key in Decoded.metrics.Keys)
            {
                var metric = metrics[key];
                if (metric != null && metric.ConvertedType == ProjectsAndMetrics.MetricType.Boolean)
                {
                    bool result;
                    bool.TryParse(Decoded.metrics[key], out result);
                    BooleanMetricResults.Add(key, result);
                }
            }
        }

        private ReleaseReportJSON Decoded { get; set; }

        public string ProjectID { get { return Decoded.project_id; } }
        public string Version { get { return Decoded.version; } }
        public Dictionary<string, bool> BooleanMetricResults { get; private set; }

        public DateTime Date
        {
            get
            {
                string year = Decoded.date.Substring(0, 4);
                string month = Decoded.date.Substring(4, 2);
                string day = Decoded.date.Substring(6, 2);

                return new DateTime(int.Parse(year), int.Parse(month), int.Parse(day));
            }
        }
    }
}
