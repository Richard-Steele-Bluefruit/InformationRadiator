using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CoreQualityMetrics.Model.Tests
{
    [TestClass]
    public class ReleaseReportTests
    {
        public const string example_data =
            "{"+
                "\"project_id\":\"project_55ca71b5b2d02\","+
                "\"version\":\"0.0.5\","+
                "\"date\":\"20150903\","+
                "\"metrics\":"+
                "{"+
                    "\"metric_55ca718c5ac18\":true,"+
                    "\"metric_55ca718c5ac19\":\"3\"," +
                    "\"metric_55ca7197b9771\":false" +
                "}"+
            "}";

        public static readonly Dictionary<string, ProjectsAndMetrics.Metric> metrics = new Dictionary<string, ProjectsAndMetrics.Metric>()
        {
            { "metric_55ca718c5ac18", new ProjectsAndMetrics.Metric() { name="", enabled = true, type=0 } },
            { "metric_55ca7197b9771", new ProjectsAndMetrics.Metric() { name="", enabled = true, type=0 } },
            { "metric_55ca718c5ac19", new ProjectsAndMetrics.Metric() { name="", enabled = true, type=1 } } // Numeric metric
        };

        [TestMethod]
        public void Decoding_report_properties()
        {
            var target = new ReleaseReport(example_data, metrics);

            Assert.AreEqual("project_55ca71b5b2d02", target.ProjectID);
            Assert.AreEqual("0.0.5", target.Version);
            Assert.AreEqual(new DateTime(2015, 09, 03), target.Date);
        }

        [TestMethod]
        public void Decoding_report_metrics()
        {
            var target = new ReleaseReport(example_data, metrics);

            Assert.AreEqual(true, target.BooleanMetricResults["metric_55ca718c5ac18"]);
            Assert.AreEqual(false, target.BooleanMetricResults["metric_55ca7197b9771"]);

            Assert.AreEqual(2, target.BooleanMetricResults.Count); // Numeric metric should be ignored
        }
    }
}
