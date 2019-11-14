using System.Collections.Generic;

namespace CoreQualityMetrics.Model
{
    public class ProjectsAndMetrics
    {

        public class Project
        {
            public string name;
        }

        public enum MetricType
        {
            Boolean,
            Numeric
        }

        public class Metric
        {
            public string name;
            public bool enabled;
            public int type;

            public MetricType ConvertedType
            {
                get
                {
                    MetricType convertedType = MetricType.Boolean;
                    MetricType.TryParse(type.ToString(), out convertedType);
                    return convertedType;
                }
            }
        }

        private class ProjectsAndMetricsJSON
        {
            public Dictionary<string, Project> projects { get; set; }
            public Dictionary<string, Metric> metrics { get; set; }
        }

        public ProjectsAndMetrics(string jsonData)
        {
            var textReader = new System.IO.StringReader(jsonData);
            var reader = new Newtonsoft.Json.JsonTextReader(textReader);
            var decode = new Newtonsoft.Json.JsonSerializer();

            DecodedProjectAndMetrics = decode.Deserialize<ProjectsAndMetricsJSON>(reader);
        }

        private ProjectsAndMetricsJSON DecodedProjectAndMetrics { get; set; }

        public Dictionary<string, Project> Projects { get { return DecodedProjectAndMetrics.projects; } }
        public Dictionary<string, Metric> Metrics { get { return DecodedProjectAndMetrics.metrics; } }
    }
}
