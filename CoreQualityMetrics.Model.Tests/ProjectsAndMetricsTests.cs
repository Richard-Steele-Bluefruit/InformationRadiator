using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace CoreQualityMetrics.Model.Tests
{
    [TestClass]
    public class ProjectsAndMetricsTests
    {
        public const string example_data =
            "{" +
                "\"projects\":"+
                "{"+
                    "\"project_55ca71b5b2d02\":{\"name\":\"Cytronex\"},"+
                    "\"project_55ca71bac656f\":{\"name\":\"ELGA - Flex\"}"+
                "},"+
                "\"metrics\":"+
                "{"+
                    "\"metric_55ca718c5ac18\":{\"name\":\"Regression Tested\",\"enabled\":true},"+
                    "\"metric_55ca718c5ac19\":{\"name\":\"Number of Tests\",\"enabled\":true,\"type\":1}," +
                    "\"metric_55ca7197b9771\":{\"name\":\"Release report\",\"enabled\":false}" +
                "}"+
            "}";

        [TestMethod]
        public void Decoding_projects()
        {
            var target = new ProjectsAndMetrics(example_data);

            Assert.IsTrue(target.Projects.Keys.Contains("project_55ca71b5b2d02"));
            Assert.AreEqual("Cytronex", target.Projects["project_55ca71b5b2d02"].name);
            Assert.AreEqual("ELGA - Flex", target.Projects["project_55ca71bac656f"].name);
        }

        [TestMethod]
        public void Decoding_metrics()
        {
            var target = new ProjectsAndMetrics(example_data);

            Assert.IsTrue(target.Metrics.Keys.Contains("metric_55ca718c5ac18"));
            Assert.AreEqual("Regression Tested", target.Metrics["metric_55ca718c5ac18"].name);
            Assert.IsTrue(target.Metrics["metric_55ca718c5ac18"].enabled);
            Assert.AreEqual(ProjectsAndMetrics.MetricType.Boolean, target.Metrics["metric_55ca718c5ac18"].ConvertedType);

            Assert.AreEqual("Release report", target.Metrics["metric_55ca7197b9771"].name);
            Assert.IsFalse(target.Metrics["metric_55ca7197b9771"].enabled);
            Assert.AreEqual(ProjectsAndMetrics.MetricType.Boolean, target.Metrics["metric_55ca7197b9771"].ConvertedType);

            Assert.AreEqual(ProjectsAndMetrics.MetricType.Numeric, target.Metrics["metric_55ca718c5ac19"].ConvertedType);
        }
    }
}
