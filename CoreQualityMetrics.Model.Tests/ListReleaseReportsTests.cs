using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace CoreQualityMetrics.Model.Tests
{
    [TestClass]
    public class ListReleaseReportsTests
    {
        private const string example_data = "{\"project_55ca71b5b2d02\":[\"0.0.5\",\"0.0.7\"],\"project_55ca71bac656f\":[\"1.2.0\",\"1.3.0\"]}";

        [TestMethod]
        public void Decoding_release_report_project_ids_and_versions()
        {
            var target = new ListReleaseReports(example_data);

            Assert.AreEqual(2, target.ReleaseReports.Keys.Count);

            Assert.AreEqual(2, target.ReleaseReports["project_55ca71b5b2d02"].Length);
            Assert.IsTrue(target.ReleaseReports["project_55ca71b5b2d02"].Contains("0.0.5"));
            Assert.IsTrue(target.ReleaseReports["project_55ca71b5b2d02"].Contains("0.0.7"));

            Assert.AreEqual(2, target.ReleaseReports["project_55ca71bac656f"].Length);
            Assert.IsTrue(target.ReleaseReports["project_55ca71bac656f"].Contains("1.2.0"));
            Assert.IsTrue(target.ReleaseReports["project_55ca71bac656f"].Contains("1.3.0"));
        }
    }
}
