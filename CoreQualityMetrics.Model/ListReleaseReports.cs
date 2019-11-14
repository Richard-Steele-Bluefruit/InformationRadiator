using System.Collections.Generic;

namespace CoreQualityMetrics.Model
{
    public class ListReleaseReports
    {
        public ListReleaseReports(string jsonData)
        {
            var textReader = new System.IO.StringReader(jsonData);
            var reader = new Newtonsoft.Json.JsonTextReader(textReader);
            var decode = new Newtonsoft.Json.JsonSerializer();

            ReleaseReports = decode.Deserialize<Dictionary<string, string[]>>(reader);
        }

        public Dictionary<string, string[]> ReleaseReports { get; private set; }
    }
}
